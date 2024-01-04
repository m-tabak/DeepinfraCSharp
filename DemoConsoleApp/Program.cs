using DemoConsoleApp;
using System.Text.Json;
using DeepinfraCSharp;


SecretConfig secretConfig = new SecretConfig();
string? apiKey = secretConfig["DeepinfraApiKey"];
if (string.IsNullOrEmpty(apiKey))
{
    throw new ArgumentNullException(nameof(apiKey));
}


DeepinfraRequestHandler requsetHandler = new(apiKey);




async Task RequestSingleResponseAsync(DeepinfraRequestHandler requsetHandler)
{

    var deepinfraResponse = await requsetHandler.RequestSingleResponseAsync("just say hi!");
    if (deepinfraResponse == null)
        Console.WriteLine("Error");
    else
    {
        deepinfraResponse.Results.ForEach((result) =>
        {
            Console.WriteLine(result.GeneratedText);
        });
        Console.ReadKey();
    }
}


async Task RequsetStreamResponseAsync(DeepinfraRequestHandler requsetHandler)
{
    var deepinfraResponse = await requsetHandler.RequestStreamResponseAsync("just say hi!");
    if (deepinfraResponse == null)
        Console.WriteLine("Error");
}