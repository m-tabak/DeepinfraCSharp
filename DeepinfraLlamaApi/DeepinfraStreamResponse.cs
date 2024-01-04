using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace DeepinfraCSharp
{
    internal class DeepinfraStreamResponse
    {
        internal DeepinfraStreamResponse()
        {
        }

        [JsonPropertyName("token")]
        internal Token Token { get; set; } = new();

        /// <summary>
        /// This doesn't return a value in a Stream, but gives the whole text at end-of-sequence. To get the text stream use <see cref="Token.Text"/>.
        /// </summary>
        [JsonPropertyName("generated_text")]
        internal string GeneratedText { get; set; } = "";

        [JsonPropertyName("details")]
        internal Details? Details { get; set; }

    }
    internal class Token
    {
        internal Token()
        {
        }

        [JsonPropertyName("id")]
        internal int Id { get; set; }

        /// <summary>
        /// The streamed token's text.
        /// </summary>
        [JsonPropertyName("text")]
        internal string Text { get; set; } = "";

        [JsonPropertyName("logprob")]
        internal double Logprob { get; set; }

        [JsonPropertyName("special")]
        internal bool Special { get; set; }
    }

    internal class Details
    {
        internal Details()
        {
        }

        [JsonPropertyName("finish_reason")]
        internal string FinishReason { get; set; } = "";

        [JsonPropertyName("generated_tokens")]
        internal int GeneratedTokens { get; set; }

        [JsonPropertyName("input_tokens")]
        internal int InputTokens { get; set; }

        [JsonPropertyName("seed")]
        internal double Seed { get; set; }
    }
}
