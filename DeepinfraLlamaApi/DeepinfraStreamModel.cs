using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace DeepinfraLlamaApi
{
    public class DeepinfraStreamModel
    {
        public DeepinfraStreamModel()
        {
        }

        [JsonPropertyName("token")]
        public Token Token { get; set; } = new();

        [JsonPropertyName("generated_text")]
        public string GeneratedText { get; set; } = "";

        [JsonPropertyName("details")]
        public Details? Details { get; set; }

    }
    public class Token
    {
        public Token()
        {
        }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; } = "";

        [JsonPropertyName("logprob")]
        public double Logprob { get; set; }

        [JsonPropertyName("special")]
        public bool Special { get; set; }
    }

    public class Details
    {
        public Details()
        {
        }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; } = "";

        [JsonPropertyName("generated_tokens")]
        public int GeneratedTokens { get; set; } 

        [JsonPropertyName("input_tokens")]
        public int InputTokens { get; set; }

        [JsonPropertyName("seed")]
        public long Seed { get; set; }
    }
}
