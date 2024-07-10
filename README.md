
# DeepinfraCSharp ![NuGet Version](https://img.shields.io/nuget/v/DeepinfraCSharp?logo=Nuget&link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FDeepinfraCSharp)


A simple C# .NET library to call Deepinfra's web API. It provides prompt managment and support for different models (currently only text-generation). This is an unofficial library and not endorsed by Deepinfra.


## Example

```C#
using DeepinfraCSharp;

var deepinfra = new TextGenApi("Your Deepinfra api Key", Model.Llama3_70B);
deepinfra.Prompt.SystemPrompt = "Be a helpful assistant.";
var tokenStream = deepinfra.RequsetStreamResponseAsync("What's the color of an Orange?");

await foreach(var token in tokenStream)
{
  Console.Write(token);
}
//Output: The color of an orange is, well, orange!
```

## Requirements

Framework .NET 6.0 or higher.

## Dependencies

RestSharp 110.2.0
