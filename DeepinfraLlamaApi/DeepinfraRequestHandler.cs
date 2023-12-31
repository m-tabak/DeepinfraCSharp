// Ignore Spelling: Deepinfra

using RestSharp;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace DeepinfraLlamaApi
{
    public class DeepinfraRequestHandler
    {
        private static RestClient client = new();
        private readonly string _apiKey;

        public DeepinfraRequestHandler(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<DeepinfraResponseModel?> InferAsync(string instructions)
        {
            DeepinfraRequestModel request = new()
            { 
                Input =$"[INST] {instructions} [/INST]"  
            };
            return await SendHttpRequestAsync(request);
        }

        public async Task<DeepinfraResponseModel?> SendRequestAsync(DeepinfraRequestModel request)
        {
            return await SendHttpRequestAsync(request);
        }

        private async Task<DeepinfraResponseModel?> SendHttpRequestAsync(DeepinfraRequestModel requestContent)
        {
            try
            {
                var client = new RestClient("https://api.deepinfra.com/v1");
                var request = new RestRequest("/inference/meta-llama/Llama-2-70b-chat-hf", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("User-Agent", "insomnia/8.5.1");
                request.AddHeader("Accept", "application/json");
                request.AddJsonBody(requestContent);
                RestResponse response = await client.ExecuteAsync(request);
                if (response.Content == null)
                    return null;
                return JsonSerializer.Deserialize<DeepinfraResponseModel>(response.Content);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }
    }
}
