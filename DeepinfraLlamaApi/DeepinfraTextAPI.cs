using DeepinfraLlamaApi;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
            if (model.IsAiroboros)
                AddStopWord("USER");
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
        /// Get a list of up to 4 strings that will terminate generation immediately. The word "USER" is added automatically when using an Airoboros model.
        /// </summary>
        public List<string>? StopWords { get; private set; } = null;

        /// <summary>
        ///  Adds a string to <see cref="StopWords"/> .If you add more than 4 it will overwrite the last string from the list.
        /// </summary>
        /// <param name="word"></param>
        public void AddStopWord(string word)
        {
            if (StopWords == null) StopWords = new List<string>();
            if (StopWords.Count == 4)
                StopWords[3] = word;
            else
                StopWords.Add(word);
        }

        /// <summary>
        ///  Send a request to infer a text, and get a single complete response.
        /// </summary>
        /// <returns>The generated text</returns>
        public async Task<string> RequestSingleResponseAsync(string question)
        {
            DeepinfraRequest request = new()
            {
                Input = FormatInput(question),
                //Todo add other options
            };

            DeepinfraSingleResponse? deepinfraResponse = await requsetHandler.RequestSingleResponseAsync(request);
            if (deepinfraResponse == null)
                return "";
            Chat += deepinfraResponse.Results[0].GeneratedText;
            return deepinfraResponse.Results[0].GeneratedText;
        }

        /// <summary>
        /// Send a request to infer a text, and get a stream of words as a response.
        /// </summary>
        /// <returns> An iterable list of words generated and recived as a stream.</returns>
        public async IAsyncEnumerable<string> RequsetStreamResponseAsync(string question, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            DeepinfraRequest request = new()
            {
                Input = FormatInput(question),
                //Todo add other options

            };

            IAsyncEnumerable<DeepinfraStreamResponse> streamResponses = requsetHandler.RequestStreamResponseAsync(request, cancellationToken);

            await foreach (var response in streamResponses.WithCancellation(cancellationToken))
            {
                if (IsChatMode)
                    Chat += response.Token.Text;
                yield return response.Token.Text;
            }
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
                if (_model.IsAiroboros)
                {
                    input += $"USER: {question}\nASSISTANT:";
                }
                else
                {
                    if (input == "")
                    {
                        input = $"[INST] {question} [/INST]";
                    }
                    else
                    {
                        input = $"\n\n[INST] {question} [/INST]";
                    }

                    if (input == "")
                        throw new WarningException("No prompt or question was sent with request to Deepinfra!");
                }
            }
            if (IsChatMode)
                Chat = input;
            return input;
        }
        private string FormatPrompt()
        {
            string prompt = "";
            if (_model.IsAiroboros)
            {
                //Add the system prompt.
                if (!string.IsNullOrEmpty(SystemPrompt))
                {
                    prompt += SystemPrompt + "\n";
                }
                //Add the examples.
                foreach (var key in Examples.Keys)
                {
                    prompt += $"USER: {key}\nASSISTANT: {Examples[key]}\n";
                }
            }
            else
            {
                bool firstLine = true;
                //Add the examples.
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
                //Add the system prompt.
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
            }
            return prompt;
        }
    }
}