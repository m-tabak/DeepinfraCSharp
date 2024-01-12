using DemoConsoleApp;
using System.Text.Json;
using DeepinfraCSharp;


SecretConfig secretConfig = new SecretConfig();
string? apiKey = secretConfig["DeepinfraApiKey"];
if (string.IsNullOrEmpty(apiKey))
{
    throw new ArgumentNullException(nameof(apiKey));
}

var examples = new Dictionary<string, string>();
examples.Add("What's the capital of France?", "Paris.");
examples.Add("What's the boiling temprature of water?", "100 degrees celsius at 1 atmosphere.");

var api = new DeepinfraTextAPI(apiKey, Model.MythoMax_13b)
{
    IsChatMode = true,
    SystemPrompt = "Be a helpful assistant. Be concise and short in your responces",
    Examples = examples,
};

Console.WriteLine("I'm your helpful assistant, ask me anything.");
while (true)
{
    var input = Console.ReadLine();
    if (string.IsNullOrEmpty(input))
        continue;
    try
    {
        var wordsStream = api.RequsetStreamResponseAsync(input, CancellationToken.None);
        await foreach(var word in wordsStream)
            Console.Write(word);
        Console.Write("\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
}

