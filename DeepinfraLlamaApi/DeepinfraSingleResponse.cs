using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace DeepinfraCSharp
{
    internal class DeepinfraSingleResponse
    {
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
        internal string RequestId { get; set; }

        /// <summary>
        /// A type representing the status of the inference request.
        /// </summary>
        [JsonPropertyName("inference_status")]
        internal InferenceStatus InferenceStatus { get; set; }

        /// <summary>
        /// A list of generated texts. Including the prompt.
        /// </summary>
        [JsonPropertyName("results")]
        internal List<Result> Results { get; set; }

        /// <summary>
        /// Number of generated tokens, excluding prompt.
        /// </summary>
        [JsonPropertyName("num_tokens")]
        internal int NumTokens { get; set; }
    }

    internal class Result
    {
        internal Result()
        {
        }

        [JsonPropertyName("generated_text")]
        internal string GeneratedText { get; set; } = "";

        internal override string ToString()
        {
            return GeneratedText;
        }
    }

    internal class InferenceStatus
    {
        internal InferenceStatus()
        {
            Status = "";
        }

        /// <summary>
        /// Inference status: "unknown", "queued", "running", "succeeded" or "failed".
        /// </summary>
        [JsonPropertyName("status")]
        internal string Status { get; set; }

        /// <summary>
        /// Billable runtime in millisecond.
        /// </summary>
        [JsonPropertyName("runtime_ms")]
        internal int RuntimeMs { get; set; }

        /// <summary>
        /// Estimated cost billed for the request in USD.
        /// </summary>
        [JsonPropertyName("cost")]
        internal decimal Cost { get; set; }

        /// <summary>
        /// Number of tokens generated.
        /// </summary>
        [JsonPropertyName("tokens_generated")]
        internal int TokensGenerated { get; set; }

        /// <summary>
        /// Number of input tokens.
        /// </summary>
        [JsonPropertyName("token_input")]
        internal int TokensInput { get; set; }
    }
}
