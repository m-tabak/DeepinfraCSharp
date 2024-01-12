using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace DeepinfraCSharp
{
    internal class DeepinfraSingleResponse
    {
        [JsonConstructorAttribute]
        internal DeepinfraSingleResponse()
        {
            RequestId = string.Empty;
            InferenceStatus = new();
            Results = [];
        }

        /// <summary>
        /// The request id
        /// </summary>
        [JsonPropertyName("request_id")]
        [JsonInclude]
        internal string RequestId { get; set; }

        /// <summary>
        /// A type representing the status of the inference request.
        /// </summary>
        [JsonPropertyName("inference_status")]
        [JsonInclude]
        internal InferenceStatus InferenceStatus { get; set; }

        /// <summary>
        /// A list of generated texts. Including the prompt.
        /// </summary>
        [JsonPropertyName("results")]
        [JsonInclude]
        internal List<Result> Results { get; set; }

        /// <summary>
        /// Number of generated tokens, excluding prompt.
        /// </summary>
        [JsonPropertyName("num_tokens")]
        [JsonInclude]
        internal int NumTokens { get; set; }
    }

    internal class Result
    {
        [JsonConstructorAttribute]
        internal Result()
        {
        }

        [JsonPropertyName("generated_text")]
        [JsonInclude]
        internal string GeneratedText { get; set; } = "";

        public override string ToString()
        {
            return GeneratedText;
        }
    }

    internal class InferenceStatus
    {
        [JsonConstructorAttribute]
        internal InferenceStatus()
        {
            Status = "";
        }

        /// <summary>
        /// Inference status: "unknown", "queued", "running", "succeeded" or "failed".
        /// </summary>
        [JsonPropertyName("status")]
        [JsonInclude]
        internal string Status { get; set; }

        /// <summary>
        /// Billable runtime in millisecond.
        /// </summary>
        [JsonPropertyName("runtime_ms")]
        [JsonInclude]
        internal int RuntimeMs { get; set; }

        /// <summary>
        /// Estimated cost billed for the request in USD.
        /// </summary>
        [JsonPropertyName("cost")]
        [JsonInclude]
        internal decimal Cost { get; set; }

        /// <summary>
        /// Number of tokens generated.
        /// </summary>
        [JsonPropertyName("tokens_generated")]
        [JsonInclude]
        internal int TokensGenerated { get; set; }

        /// <summary>
        /// Number of input tokens.
        /// </summary>
        [JsonPropertyName("token_input")]
        [JsonInclude]
        internal int TokensInput { get; set; }
    }
}
