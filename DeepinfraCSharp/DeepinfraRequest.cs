using DeepinfraCSharp;
using System.Text.Json.Serialization;

namespace DeepinfraLlamaApi
{
    internal class DeepinfraRequest : InferenceParamaters
    {
        [JsonConstructorAttribute]
        internal DeepinfraRequest()
        {
        }

        /// <summary>
        /// text to generate from
        /// </summary>
        [JsonPropertyName("input")]
        [JsonInclude]
        internal string Input { get; set; } = "Say Hi!";

        /// <summary>
        /// Number of output sequences to return, max 2. Incompatible with streaming.
        /// </summary>
        [JsonPropertyName("num_responses")]
        [JsonInclude]
        internal int? NumResponses { get; set; } = null;

        /// <summary>
        /// Deepinfra only accepts max 4 stop sequences.
        /// </summary>
        [JsonPropertyName("stop")]
        [JsonInclude]
        internal List<string>? Stop { get; set; } = null;

        /// <summary>
        /// The webhook to call when inference is done, by default you will get the output in the response of your inference request.
        /// </summary>
        [JsonPropertyName("webhook")]
        [JsonInclude]
        internal Uri? Webhook { get; set; } = null;

        /// <summary>
        /// Whether to stream tokens, by default it will be false, currently only supported for Llama 2 text generation models, token by token updates will be sent over SSE.
        /// </summary>
        [JsonPropertyName("stream")]
        [JsonInclude]
        internal bool? Stream { get; set; } = null;

    }
}
