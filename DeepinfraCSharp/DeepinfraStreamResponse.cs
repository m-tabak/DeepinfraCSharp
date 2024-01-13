using System.Text.Json.Serialization;

namespace DeepinfraCSharp
{
    internal class DeepinfraStreamResponse
    {
        [JsonConstructorAttribute]
        internal DeepinfraStreamResponse()
        {
        }

        [JsonPropertyName("token")]
        [JsonInclude]
        internal Token Token { get; set; } = new();

        /// <summary>
        /// This doesn't return a value in a Stream, but gives the whole text at end-of-sequence. To get the text stream use <see cref="Token.Text"/>.
        /// </summary>
        [JsonPropertyName("generated_text")]
        [JsonInclude]
        internal string GeneratedText { get; set; } = "";

        [JsonPropertyName("details")]
        [JsonInclude]
        internal Details? Details { get; set; }

    }
    internal class Token
    {
        [JsonConstructorAttribute]
        internal Token()
        {
        }

        [JsonPropertyName("id")]
        [JsonInclude]
        internal int? Id { get; set; }

        /// <summary>
        /// The streamed token's text.
        /// </summary>
        [JsonPropertyName("text")]
        [JsonInclude]
        internal string Text { get; set; } = "";

        [JsonPropertyName("logprob")]
        [JsonInclude]
        internal double Logprob { get; set; }

        [JsonPropertyName("special")]
        [JsonInclude]
        internal bool Special { get; set; }
    }

    internal class Details
    {
        [JsonConstructorAttribute]
        internal Details()
        {
        }

        [JsonPropertyName("finish_reason")]
        [JsonInclude]
        internal string FinishReason { get; set; } = "";

        [JsonPropertyName("generated_tokens")]
        [JsonInclude]
        internal int GeneratedTokens { get; set; }

        [JsonPropertyName("input_tokens")]
        [JsonInclude]
        internal int InputTokens { get; set; }

        [JsonPropertyName("seed")]
        [JsonInclude]
        internal double Seed { get; set; }
    }
}
