using DeepinfraLlamaApi;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace DeepinfraCSharp
{
    public class DeepinfraTextAPI
    {
        private readonly DeepinfraRequestHandler requsetHandler;
        private readonly Model _model;

        public DeepinfraTextAPI(string apiKey, Model model)
        {
            requsetHandler = new(apiKey, model);
            _model = model;
            switch (model.PromptStyle)
            {
                case PromptFormat.Airoboros:
                    StopWords = new() { "USER" };
                    break;
                case PromptFormat.Llama:
                    StopWords = new() { "[INST" };
                    break;
                case PromptFormat.Alpaca:
                    StopWords = new() { "###" };
                    break;
            }
        }

        /// <summary>
        /// Examples to give in the prompt.
        /// The (key, value) pair represents (Question, Answer).
        /// </summary>
        public Dictionary<string, string> Examples { get; set; } = new();

        /// <summary>
        /// A special messages to steer the behavior of the large langauge model.
        /// </summary>
        public string SystemPrompt { get; set; } = string.Empty;

        /// <summary>
        /// Whether to send the previous questions and answers with the requests. Defult = false
        /// </summary>
        public bool IsChatMode = false;

        /// <summary>
        /// The current conversation, including the prompt. Only works if <see cref="IsChatMode"/> is set to true.
        /// </summary>
        public string ChatText { get; set; } = string.Empty;

        /// <summary>
        /// Parameters used in generating text. 
        /// If no value was assigned to a parameter, Deepinfra uses a default value. 
        /// For more information about default values see https://deepinfra.com/meta-llama/Llama-2-70b-chat-hf/api#input-max_new_tokens/>
        /// </summary>
        public InferenceParamaters Paramaters { get; set; } = new();

        /// <summary>
        /// Strings that will terminate generation immediately. Only the first 4 strings will be sent with the request.
        /// </summary>
        public List<string>? StopWords { get; set; }

        /// <summary>
        ///  Send a request to infer a text, and get a single complete response.
        /// </summary>
        /// <returns>The generated text</returns>
        public async Task<string> RequestSingleResponseAsync(string question)
        {
            var request = GetNewRequest(question);

            DeepinfraSingleResponse? deepinfraResponse = await requsetHandler.RequestSingleResponseAsync(request);
            if (deepinfraResponse == null)
                return "";
            if (deepinfraResponse.Results.Count <= 0)
                throw new Exception("Nko tokens where generated in the response");
            var generatedText = deepinfraResponse.Results[0].GeneratedText;
            //remove the stop sequence from the end of the generated text, if it exists.
            if (StopWords is not null)
                foreach (var stopWord in StopWords)
                {
                    if (generatedText.EndsWith(stopWord))
                        generatedText = generatedText.Remove(generatedText.Length - stopWord.Length + 1);
                }
            if (IsChatMode)
                ChatText += generatedText;
            return generatedText;
        }

        /// <summary>
        /// Send a request to infer a text, and get a stream of words as a response.
        /// </summary>
        /// <returns> An iterable list of tokens.</returns>
        public async IAsyncEnumerable<string> RequsetStreamResponseAsync(string question, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var request = GetNewRequest(question);

            IAsyncEnumerable<DeepinfraStreamResponse> streamResponses = requsetHandler.RequestStreamResponseAsync(request, cancellationToken);

            await foreach (var response in streamResponses.WithCancellation(cancellationToken))
            {
                if (StopWords is not null && StopWords.Contains(response.Token.Text))
                    yield break;
                if (IsChatMode)
                    ChatText += response.Token.Text;
                yield return response.Token.Text;
            }
        }

        private DeepinfraRequest GetNewRequest(string question)
        {
            //Deepinfra only accepts 4 stop sequences.
            if (StopWords is not null && StopWords.Count > 4)
            {
                StopWords = StopWords.Take(4).ToList();
            }

            return new DeepinfraRequest()
            {
                Input = FormatInput(question),
                Stop = StopWords,
                Temperature = Paramaters.Temperature,
                MaxNewTokens = Paramaters.MaxNewTokens,
                TopK = Paramaters.TopK,
                TopP = Paramaters.TopP,
                RepetitionPenalty = Paramaters.RepetitionPenalty,
                FrequencyPenalty = Paramaters.FrequencyPenalty,
                PresencePenalty = Paramaters.PresencePenalty,
            };
        }

        private string FormatInput(string question)
        {
            string input = "";
            if (IsChatMode && ChatText != string.Empty)
            {
                input = ChatText;
                if (!input.EndsWith('\n'))
                    input += "\n";
            }
            else
            {
                input = FormatPrompt();
            }

            //Add the last question
            if (!string.IsNullOrEmpty(question))
            {
                switch (_model.PromptStyle)
                {
                    case PromptFormat.Airoboros:
                        input += $"USER: {question}\nASSISTANT:";
                        break;
                    case PromptFormat.Llama:
                        if (!input.EndsWith("\n"))
                            input += "\n";
                        input += $"[INST] {question} [/INST]";
                        break;
                    case PromptFormat.Alpaca:
                        input += $"### Instruction:\n\n{question}\n\n### Response:";
                        break;
                }
                if (input == "")
                    throw new WarningException("No prompt or question was sent with request to Deepinfra!");
            }
            if (IsChatMode)
                ChatText = input;
            return input;
        }

        private string FormatPrompt()
        {
            string prompt = "";
            switch (_model.PromptStyle)
            {
                case PromptFormat.Airoboros:
                    //system prompt.
                    if (!string.IsNullOrEmpty(SystemPrompt))
                    {
                        prompt += SystemPrompt + "\n";
                    }
                    //examples.
                    foreach (var key in Examples.Keys)
                    {
                        prompt += $"USER: {key}\nASSISTANT: {Examples[key]}\n";
                    }
                    break;
                case PromptFormat.Llama:
                    bool firstLine = true;
                    //examples.
                    foreach (var key in Examples.Keys)
                    {
                        if (firstLine)
                        {
                            prompt = $"[INST] {key} [/INST] {Examples[key]}";
                            firstLine = false;
                        }
                        else
                        {
                            prompt += $"</s><s>\n[INST] {key} [/INST] {Examples[key]}";
                        }
                    }
                    //system prompt.
                    if (!string.IsNullOrEmpty(SystemPrompt))
                    {
                        if (firstLine)
                        {
                            prompt = $"[INST] <<SYS>>\n{SystemPrompt}\n<<SYS>>\n\n[/INST]";
                            firstLine = false;
                        }
                        else
                        {
                            prompt = prompt.Insert(7, $"<<SYS>>\n{SystemPrompt}\n<<SYS>>\n\n");
                        }
                    }
                    break;
                case PromptFormat.Alpaca:
                    //system prompt.
                    if (!string.IsNullOrEmpty(SystemPrompt))
                    {
                        prompt += $"{SystemPrompt}\n\n";
                    }
                    //examples.
                    foreach (var key in Examples.Keys)
                    {
                        prompt += $"### Instruction: {key}\n\n### Response: {Examples[key]}\n";
                    }
                    break;
            }
            return prompt;
        }
    }
}