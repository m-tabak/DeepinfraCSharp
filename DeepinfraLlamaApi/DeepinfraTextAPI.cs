using DeepinfraLlamaApi;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
        /// A string that stores the current the prompt, questons and answers. Only works if <see cref="IsChatMode"/> is set to true.
        /// </summary>
        public string Chat { get; set; } = string.Empty;

        /// <summary>
        /// temperature to use for sampling. 0 means the output is deterministic. Values greater than 1 encourage more diversity. If left to be null, Deepinfra applies a default value.
        /// </summary>
        public double? Temprature { get; set; } = null;

        /// <summary>
        /// Maximum length of the newly generated text. If left to be null, Deepinfra applies a default value.
        /// </summary>
        public int? MaxNewTokens { get; set; } = null;

        /// <summary>
        /// Sample from the set of tokens with highest probability such that sum of probabilities is higher than p. Values from 0 to 1, lower values focus on the most probable tokens.Higher values sample more low-probability tokens. If left to be null, Deepinfra applies a default value.
        /// </summary>
        public double? TopP { get; set; } = null;

        /// <summary>
        /// Sample from the best k (number of) tokens. 0 means off, max is 100000. If left to be null, Deepinfra applies a default value.
        /// </summary>
        public double? TopK { get; set; } = null;

        /// <summary>
        /// Values from 0.01 to 5. Value of 1 means no penalty, values greater than 1 discourage repetition, smaller than 1 encourage repetition. If left to be null, Deepinfra applies a default value.
        /// </summary>
        public double? RepetitionPenalty { get; set; } = null;

        /// <summary>
        /// Values form -2 to +2. Positive values penalize new tokens based on whether they appear in the text so far, increasing the model's likelihood to talk about new topics. If left to be null, Deepinfra applies a default value.
        /// </summary>
        public double? PresencePenalty { get; set; } = null;

        /// <summary>
        /// Values form -2 to +2. Positive values penalize new tokens based on how many times they appear in the text so far, increasing the model's likelihood to talk about new topics. If left to be null, Deepinfra applies a default value.
        /// </summary>
        public double? FrequencyPenalty { get; set; } = null;

        /// <summary>
        /// A list of strings that will terminate generation immediately. Only the first 4 items get sent with the request. "USER" or "[INST]" is added automatically depinding on the model.
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
                Chat += generatedText;
            return generatedText;
        }

        /// <summary>
        /// Send a request to infer a text, and get a stream of words as a response.
        /// </summary>
        /// <returns> An iterable list of words and characters generated and recived as a stream.</returns>
        public async IAsyncEnumerable<string> RequsetStreamResponseAsync(string question, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var request = GetNewRequest(question);

            IAsyncEnumerable<DeepinfraStreamResponse> streamResponses = requsetHandler.RequestStreamResponseAsync(request, cancellationToken);

            await foreach (var response in streamResponses.WithCancellation(cancellationToken))
            {
                if (StopWords is not null && StopWords.Contains(response.Token.Text))
                    yield break;
                if (IsChatMode)
                    Chat += response.Token.Text;
                yield return response.Token.Text;
            }
        }

        private DeepinfraRequest GetNewRequest(string question)
        {
            //Deepinfra only accepts 4 stop sequences.
            if (StopWords is not null && StopWords.Count > 4)
            {
                StopWords = StopWords.Slice(0, 4);
            }

            return new DeepinfraRequest()
            {
                Input = FormatInput(question),
                Stop = StopWords,
                Temperature = Temprature,
                MaxNewTokens = MaxNewTokens,
                TopK = TopK,
                TopP = TopP,
                RepetitionPenalty = RepetitionPenalty,
                FrequencyPenalty = FrequencyPenalty,
                PresencePenalty = PresencePenalty,
            };
        }

        private string FormatInput(string question)
        {
            string input = "";
            if (IsChatMode && Chat != string.Empty)
            {
                input = Chat;
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
                Chat = input;
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