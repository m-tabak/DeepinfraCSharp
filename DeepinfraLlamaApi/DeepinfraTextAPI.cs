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
        }

        /// <summary>
        /// Examples to give in the prompt.
        //  Set the key to a question, and set the value to an answer.
        /// </summary>
        public Dictionary<string, string> Examples { get; set; } = new();

        /// <summary>
        /// A special messages to steer the behavior of the large langauge model.
        /// </summary>
        public string SystemPrompt { get; set; } = "";

        /// <summary>
        /// Whether to send the previous questions and answers with the requests. Defult = false
        /// </summary>
        public bool isChatMode = false;

        /// <summary>
        /// Up to 4 strings that will terminate generation immediately. eg. "User".
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
                Input = FormatPrompt(question),
                //Todo add other options
            };

            DeepinfraSingleResponse? deepinfraResponse = await requsetHandler.RequestSingleResponseAsync(request);
            if (deepinfraResponse == null)
                return "";
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
                Input = FormatPrompt(question),
                //Todo add other options

            };

            IAsyncEnumerable<DeepinfraStreamResponse> streamResponses = requsetHandler.RequestStreamResponseAsync(request, cancellationToken);

            await foreach (var response in streamResponses.WithCancellation(cancellationToken))
            {
                yield return response.Token.Text;
            }


        }
        private string FormatPrompt(string question)
        {
            string result = "";
            bool firstLine = true;
            if (_model.IsAiroboros)
            {
                // TODO Airboros prompt
                return result;
            }
            else
            {
                //Add the examples.
                foreach (var key in Examples.Keys)
                {
                    if (firstLine)
                    {
                        result = $"[INST] {key} [/ INST] {Examples[key]}";
                        firstLine = false;
                    }
                    else
                    {
                        result += $"</s><s>\n[INST] {key} [/ INST] {Examples[key]}";
                    }

                }
                //Add the system prompt.
                if (!string.IsNullOrEmpty(SystemPrompt))
                {
                    if (firstLine)
                    {
                        result = $"[INST] <<SYS>>\n{SystemPrompt}\n<<SYS>>\n\n[/INST]";
                        firstLine = false;
                    }
                    else
                    {
                        result = result.Insert(7, $"<<SYS>>\n{SystemPrompt}\n<<SYS>>\n\n");
                    }
                }
                //Add the final instruction/question
                if (!string.IsNullOrEmpty(question))
                    if (firstLine)
                    {
                        result = $"[INST] {question} [/ INST]";
                        firstLine = false;
                    }
                    else
                    {
                        result = $"\n\n[INST] {question} [/ INST]";
                    }

                if (result == "")
                    throw new WarningException("No prompt or question was sent with request to Deepinfra!");
                return result;
            }
        }
    }
}