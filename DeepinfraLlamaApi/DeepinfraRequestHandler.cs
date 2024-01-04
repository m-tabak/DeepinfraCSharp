// Ignore Spelling: Deepinfra

using DeepinfraLlamaApi;
using RestSharp;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace DeepinfraCSharp
{
    internal class DeepinfraRequestHandler
    {
        private static RestClient client = new RestClient("https://api.deepinfra.com/v1");
        private readonly string _apiKey;

        internal DeepinfraRequestHandler(string apiKey)
        {
            _apiKey = apiKey;
        }

        internal async Task<DeepinfraSingleResponse?> RequestSingleResponseAsync(DeepinfraRequest requestContent)
        {
            var request = GetRequest(requestContent);
            return await SendRequestAsync(request);
        }

        internal async IAsyncEnumerable<DeepinfraStreamResponse> RequestStreamResponseAsync(
            DeepinfraRequest requestContent,
            [EnumeratorCancellation] CancellationToken cancellationToken = default
        )
        {
            var request = GetRequest(requestContent);
            var stream = await client.DownloadStreamAsync(request, cancellationToken);
            if (stream == null)
                yield break;

            using var reader = new StreamReader(stream);
            while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                line = line.Substring(line.IndexOf('{'));
                yield return JsonSerializer.Deserialize<DeepinfraStreamResponse>(line)!;
            }
        }

        private async Task<DeepinfraSingleResponse?> SendRequestAsync(RestRequest request)
        {
            RestResponse response = await client.ExecuteAsync(request);
            if (response.Content == null)
                return null;
            return JsonSerializer.Deserialize<DeepinfraSingleResponse>(response.Content);
        }

        private RestRequest GetRequest(DeepinfraRequest requestContent)
        {
            var request = new RestRequest("/inference/meta-llama/Llama-2-70b-chat-hf", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", $"Bearer {_apiKey}");
            request.AddJsonBody(requestContent);
            return request;
        }
    }
}
