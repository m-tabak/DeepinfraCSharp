
# DeepinfraCSharp ![NuGet Version](https://img.shields.io/nuget/v/DeepinfraCSharp?logo=Nuget&link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FDeepinfraCSharp)


A simple C# .NET library to call Deepinfra's web API. It provides prompt managment and support for different models (currently only text-generation). This is an unofficial library and not endorsed by Deepinfra.


## Example

```C#
using DeepinfraCSharp;

var api = new DeepinfraTextAPI(apiKey, Model.Llama3_70B);
api.Prompt.SystemPrompt = "Be a helpful assistant.";
var wordsStream = api.RequsetStreamResponseAsync("What's the color of an Orange?");

await foreach(var word in wordsStream)
{
  Console.Write(word);
}
//Output: The color of an orange is, well, orange! Orange is a warm, vibrant color that is named after the fruit. It is a mixture of red and yellow, and is often associated with energy, enthusiasm, and creativity.
```

## Requirements

Framework .NET 6.0 or higher.

## Dependencies

RestSharp 110.2.0
