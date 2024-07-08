using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace DeepinfraCSharp
{
    /// <summary>
    /// An API for managing requsets/responses to Deepinfra.
    /// </summary>
    public class DeepinfraTextAPI
    {
        private readonly DeepinfraRequestHandler requsetHandler;
        private readonly Model _model;

        /// <summary>
        /// </summary>
        /// <param name="apiKey">Deepinfra's authentication key</param>
        /// <param name="model">The large language model. A static property of the type <see cref="Model"/>. Example: <see cref="Model.Airoboros_70b"/>></param>
        public DeepinfraTextAPI(string apiKey, Model model)
        {
            requsetHandler = new(apiKey, model);
            _model = model;
            switch (model.PromptStyle)
            {
                case PromptFormat.Airoboros:
                    StopWords = new() { "USER" };
                    break;
                case PromptFormat.Llama2:
                    StopWords = new() { "[INST" };
                    break;
                case PromptFormat.Alpaca:
                    StopWords = new() { "###" };
                    break;
            }
        }

        /// <summary>
        /// The promp that gets send to the model. 
        /// </summary>
        public PromptSettings Prompt { get; set; } = new();

        /// <summary>
        /// If enabled, previous interactions in the conversation get sent with requests.
        /// </summary>
        public ChatInformation Chat { get; set; } = new();

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
        /// <param name="question">A question to answer or text to complete.</param>
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
            if (this.Chat.Enabled)
                Chat.conversation.Add(new() { Input = question, Output = generatedText });
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
            if (this.Chat.Enabled)
                Chat.conversation.Add(new() { Input = question, Output = "" });

            await foreach (var response in streamResponses.WithCancellation(cancellationToken))
            {
                if (StopWords is not null && StopWords.Contains(response.Token.Text))
                    yield break;
                if (this.Chat.Enabled)
                    //Add the word to the output of the last interaction
                    Chat.conversation[Chat.conversation.Count - 1].Output += response.Token.Text;
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
            input = FormatPrompt();

            if (Chat.Enabled && Chat.Conversation.Any())
            {
                foreach (var interaction in Chat.conversation)
                {
                    input += interaction.FormatInteraction(_model);
                }
            }

            //Add the last question
            if (!string.IsNullOrEmpty(question))
            {
                switch (_model.PromptStyle)
                {
                    case PromptFormat.Airoboros:
                        input += $"USER: {question}\nASSISTANT:";
                        break;
                    case PromptFormat.Llama2:
                        if (!input.EndsWith("\n"))
                            input += "\n";
                        input += $"[INST] {question} [/INST]";
                        break;
                    case PromptFormat.Alpaca:
                        input += $"### Instruction:\n\n{question}\n\n### Response:";
                        break;
                    case PromptFormat.Llama3:
                        input += $"<|start_header_id|>user<|end_header_id|>\n{question}<|eot_id|><|start_header_id|>assistant<|end_header_id|>\n";
                        break;
                }
                if (input == "")
                    throw new WarningException("No prompt or question was sent with request to Deepinfra!");
            }
            return input;
        }

        private string FormatPrompt()
        {
            string prompt = "";
            if (_model.PromptStyle == PromptFormat.Llama3)
                prompt = "<|begin_of_text|>";

            if (!string.IsNullOrEmpty(Prompt.SystemPrompt))
                switch (_model.PromptStyle)
                {
                    case PromptFormat.Airoboros:
                        prompt = Prompt.SystemPrompt + "\n";
                        break;
                    case PromptFormat.Alpaca:
                        prompt = $"{Prompt.SystemPrompt}\n\n";
                        break;
                    case PromptFormat.Llama3:
                        prompt += $"<|start_header_id|>system<|end_header_id|>\n{Prompt.SystemPrompt}<|eot_id|>";
                        break;
                }
            foreach (var example in Prompt.InteractionExamples)
            {
                prompt += example.FormatInteraction(_model);
            }
            return prompt;
        }
    }
}