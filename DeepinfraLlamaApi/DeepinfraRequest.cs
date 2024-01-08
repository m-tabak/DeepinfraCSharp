using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace DeepinfraLlamaApi
{
    internal class DeepinfraRequest
    {
        /// <summary>
        /// text to generate from
        /// </summary>
        [JsonPropertyName("input")]
        internal required string Input { get; set; }

        /// <summary>
        /// maximum length of the newly generated text. Default is 2048
        /// </summary>
        [JsonPropertyName("max_new_tokens")]
        internal int? MaxNewTokens { get; set; } = null;

        /// <summary>
        /// temperature to use for sampling. 0 means the output is deterministic. Values greater than 1 encourage more diversity
        /// </summary>
        [JsonPropertyName("temperature")]
        internal double? Temperature { get; set; } = null;

        /// <summary>
        /// Sample from the set of tokens with highest probability such that sum of probabilities is higher than p. Values from 0 to 1, lower values focus on the most probable tokens.Higher values sample more low-probability tokens
        /// </summary>
        [JsonPropertyName("top_p")]
        internal double? TopP { get; set; } = null;

        /// <summary>
        /// Sample from the best k (number of) tokens. 0 means off, max is 100000.
        /// </summary>
        [JsonPropertyName("top_k")]
        internal double? TopK { get; set; } = null;

        /// <summary>
        /// Values from 0.01 to 5. Value of 1 means no penalty, values greater than 1 discourage repetition, smaller than 1 encourage repetition.
        /// </summary>
        [JsonPropertyName("repetition_penalty")]
        internal double? RepetitionPenalty { get; set; } = null;

        /// <summary>
        /// Up to 4 strings that will terminate generation immediately.
        /// </summary>
        [JsonPropertyName("stop")]
        internal List<string>? Stop { get; set; } = null;

        /// <summary>
        /// Number of output sequences to return, max 2. Incompatible with streaming.
        /// </summary>
        [JsonPropertyName("num_responses")]
        internal int? NumResponses { get; set; } = null;

        /// <summary>
        /// Values form -2 to +2. Positive values penalize new tokens based on whether they appear in the text so far, increasing the model's likelihood to talk about new topics.
        /// </summary>
        [JsonPropertyName("presence_penalty")]
        internal double? PresencePenalty { get; set; } = null;

        /// <summary>
        /// Values form -2 to +2. Positive values penalize new tokens based on how many times they appear in the text so far, increasing the model's likelihood to talk about new topics.
        /// </summary>
        [JsonPropertyName("frequency_penalty")]
        internal double? FrequencyPenalty { get; set; } = null;

        /// <summary>
        /// The webhook to call when inference is done, by default you will get the output in the response of your inference request.
        /// </summary>
        [JsonPropertyName("webhook")]
        internal Uri? Webhook { get; set; } = null;

        /// <summary>
        /// Whether to stream tokens, by default it will be false, currently only supported for Llama 2 text generation models, token by token updates will be sent over SSE.
        /// </summary>
        [JsonPropertyName("stream")]
        internal bool? Stream { get; set; } = null;

        internal DeepinfraRequest()
        {
        }

    }
}
