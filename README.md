
# DeepinfraCSharp

A simple C# .NET library to call Deepinfra's web API. It provides prompt managment and support for different models (currently only text-generation). This is an unofficial library and not endorsed by Deepinfra.


## Example

```C#
using DeepinfraCSharp;

var api = new DeepinfraTextAPI("Api_Key", Model.Airoboros_70b)
{
    SystemPrompt = "Be a helpful assistant.",
};
var wordsStream = api.RequsetStreamResponseAsync("What's the color of an Orange?");
await foreach(var word in wordsStream)
{
  Console.Write(word);
}
//Output: The color of an Orange is Orange.
```
## Requirements

Framework .NET 6.0 or higher.
