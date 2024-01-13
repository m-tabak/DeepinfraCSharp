using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeepinfraCSharp
{
    public class InferenceParamaters
    {
        private int? maxNewTokens = null;
        private double? temperature = null;
        private double? topP = null;
        private double? topK = null;
        private double? repetitionPenalty = null;
        private double? presencePenalty = null;
        private double? frequencyPenalty = null;

        public InferenceParamaters() { }

        /// <summary>
        /// maximum length of the newly generated text. Values between 1 and 100000
        /// </summary>
        [JsonPropertyName("max_new_tokens")]
        [JsonInclude]
        public int? MaxNewTokens
        {
            get => maxNewTokens;
            set
            {
                if (value < 1) maxNewTokens = 1;
                if (value > 100000) maxNewTokens = 100000;
            }
        }

        /// <summary>
        /// temperature to use for sampling. 0 means the output is deterministic. Values greater than 1 encourage more diversity. max 100
        /// </summary>
        [JsonPropertyName("temperature")]
        [JsonInclude]
        public double? Temperature
        {
            get => temperature;
            set
            {
                if (value < 0) temperature = 0;
                if (value > 100) temperature = 100;
            }
        }

        /// <summary>
        /// Sample from the set of tokens with highest probability such that sum of probabilities is higher than p. Values from 0 to 1, lower values focus on the most probable tokens.Higher values sample more low-probability tokens
        /// </summary>
        [JsonPropertyName("top_p")]
        [JsonInclude]
        public double? TopP
        {
            get => topP;
            set
            {
                if (value < 0) topP = 0;
                if (value > 1) topP = 1;
            }
        }

        /// <summary>
        /// Sample from the best k (number of) tokens. Values from 0 to 100000. 0 means off.
        /// </summary>
        [JsonPropertyName("top_k")]
        [JsonInclude]
        public double? TopK
        {
            get => topK;
            set
            {
                if (value < 0) topK = 0;
                if (value > 100000) topK = 100000;
            }
        }

        /// <summary>
        /// Value of 1 means no penalty, values greater than 1 discourage repetition, smaller than 1 encourage repetition. Values from 0.01 to 5.
        /// </summary>
        [JsonPropertyName("repetition_penalty")]
        [JsonInclude]
        public double? RepetitionPenalty
        {
            get => repetitionPenalty;
            set
            {
                if (value < 0.01) repetitionPenalty = 0.01;
                if (value > 5) repetitionPenalty = 5;
            }
        }

        /// <summary>
        ///  Positive values penalize new tokens based on whether they appear in the text so far, increasing the model's likelihood to talk about new topics. Values form -2 to +2.
        /// </summary>
        [JsonPropertyName("presence_penalty")]
        [JsonInclude]
        public double? PresencePenalty
        {
            get => presencePenalty;
            set
            {
                if (value < -2) presencePenalty = -2;
                if (value > 2) presencePenalty = 2;
            }
        }

        /// <summary>
        /// Positive values penalize new tokens based on how many times they appear in the text so far, increasing the model's likelihood to talk about new topics. Values form -2 to +2.
        /// </summary>
        [JsonPropertyName("frequency_penalty")]
        [JsonInclude]
        public double? FrequencyPenalty
        {
            get => frequencyPenalty;
            set
            {
                if (value < -2) frequencyPenalty = -2;
                if (value > 2) frequencyPenalty = 2;
            }
        }
    }
}
