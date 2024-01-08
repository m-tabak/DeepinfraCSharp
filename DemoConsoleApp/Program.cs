using DemoConsoleApp;
using System.Text.Json;
using DeepinfraCSharp;


SecretConfig secretConfig = new SecretConfig();
string? apiKey = secretConfig["DeepinfraApiKey"];
if (string.IsNullOrEmpty(apiKey))
{
    throw new ArgumentNullException(nameof(apiKey));
}

