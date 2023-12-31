using DemoConsoleApp;
using DeepinfraLlamaApi;
using System.Text.Json;


SecretConfig secretConfig = new SecretConfig();
string? apiKey = secretConfig["DeepinfraApiKey"];
if (string.IsNullOrEmpty(apiKey))
{
    throw new ArgumentNullException(nameof(apiKey));
}

DeepinfraRequestHandler requsetHandler = new(apiKey);
var deepinfraResponse = await requsetHandler.InferAsync("just say hi!");
if (deepinfraResponse == null)
    Console.WriteLine("Error");
else
{
string results = string.Join('\n', deepinfraResponse.Results);

Console.WriteLine(results);
Console.ReadKey();
}


