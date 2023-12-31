using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeepinfraLlamaApi
{
    public class DeepinfraResponseModel
    {
        public DeepinfraResponseModel()
        {
            RequestId = string.Empty;
            InferenceStatus = new();
            Results = [];
        }

        /// <summary>
        /// The request id
        /// </summary>
        [JsonPropertyName("request_id")]
        public string RequestId { get; set; }

        /// <summary>
        /// A type representing the status of the inference request.
        /// </summary>
        [JsonPropertyName("inference_status")]
        public InferenceStatus InferenceStatus { get; set; }

        /// <summary>
        /// A list of generated texts. Including the prompt.
        /// </summary>
        [JsonPropertyName("results")]
        public List<Result> Results { get; set; }

        /// <summary>
        /// Number of generated tokens, excluding prompt.
        /// </summary>
        [JsonPropertyName("num_tokens")]
        public int NumTokens { get; set; }
    }

    public class Result
    {
        public Result()
        {
        }

        [JsonPropertyName("generated_text")]
        public string GeneratedText { get; set; } = "";

        public override string ToString()
        {
            return GeneratedText;
        }
    }

    public class InferenceStatus
    {
        public InferenceStatus()
        {
            Status = "";
        }

        /// <summary>
        /// Inference status: "unknown", "queued", "running", "succeeded" or "failed".
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Billable runtime in millisecond.
        /// </summary>
        [JsonPropertyName("runtime_ms")]
        public int RuntimeMs { get; set; }

        /// <summary>
        /// Estimated cost billed for the request in USD.
        /// </summary>
        [JsonPropertyName("cost")]
        public decimal Cost { get; set; }

        /// <summary>
        /// Number of tokens generated.
        /// </summary>
        [JsonPropertyName("tokens_generated")]
        public int TokensGenerated { get; set; }

        /// <summary>
        /// Number of input tokens.
        /// </summary>
        [JsonPropertyName("token_input")]
        public int TokensInput {  get; set; }
    }
}
