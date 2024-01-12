// Ignore Spelling: Deepinfra

using DeepinfraLlamaApi;
using RestSharp;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeepinfraCSharp
{
    internal class DeepinfraRequestHandler
    {
        private static RestClient client = new RestClient("https://api.deepinfra.com/v1/inference/");
        private readonly string _apiKey;
        private readonly string _endpoint;

        internal DeepinfraRequestHandler(string apiKey, string endpoint)
        {
            _apiKey = apiKey;
            _endpoint = endpoint;
        }

        internal async Task<DeepinfraSingleResponse?> RequestSingleResponseAsync(DeepinfraRequest requestContent)
        {
            var request = GetRequest(requestContent);
            RestResponse response = await client.ExecuteAsync(request);
            //Exceptions
            if (response.ErrorException != null)
                throw new Exception(response.ErrorException.Message + Log(request, response));
            if (!string.IsNullOrEmpty(response.ErrorMessage))
                throw new Exception(response.ErrorMessage + Log(request, response));

            return JsonSerializer.Deserialize<DeepinfraSingleResponse>(response.Content!);
        }

        internal async IAsyncEnumerable<DeepinfraStreamResponse> RequestStreamResponseAsync(
            DeepinfraRequest requestContent,
            [EnumeratorCancellation] CancellationToken cancellationToken = default
        )
        {
            requestContent.Stream = true;
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
                var indexOfJson = line.IndexOf('{');
                if (indexOfJson != -1)
                {
                    line = line.Substring(indexOfJson);
                    yield return JsonSerializer.Deserialize<DeepinfraStreamResponse>(line)!;
                }
            }
        }

        private RestRequest GetRequest(DeepinfraRequest requestContent)
        {
            var request = new RestRequest(_endpoint, Method.Post);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", $"Bearer {_apiKey}");
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            var json = JsonSerializer.Serialize<DeepinfraRequest>(requestContent, options);
            request.AddBody(json, ContentType.Json);
            return request;
        }

        private string Log(RestRequest request, RestResponse response)
        {
            var requestToLog = new
            {
                resource = request.Resource,
                // Parameters are custom anonymous objects in order to have the parameter type as a nice string
                // otherwise it will just show the enum value
                parameters = request.Parameters.Select(parameter => new
                {
                    name = parameter.Name,
                    value = parameter.Value,
                    type = parameter.Type.ToString()
                }),
                // ToString() here to have the method as a nice string otherwise it will just show the enum value
                method = request.Method.ToString(),
                // This will generate the actual Uri used in the request
                uri = client.BuildUri(request),
            };

            var responseToLog = new
            {
                statusCode = response.StatusCode,
                content = response.Content,
                headers = response.Headers,
                // The Uri that actually responded (could be different from the requestUri if a redirection occurred)
                responseUri = response.ResponseUri,
                errorMessage = response.ErrorMessage,
            };

            return string.Format(" \nRequest: {0}\n Response: {1}",
                    JsonSerializer.Serialize(requestToLog),
                    JsonSerializer.Serialize(responseToLog));
        }
    }
}