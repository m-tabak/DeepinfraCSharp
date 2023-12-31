using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DemoConsoleApp
{
    internal class SecretConfig
    {
        internal string? this[string key]
        {
            get
            {
                using (StreamReader r = new StreamReader("secrets.json"))
                {
                    string json = r.ReadToEnd();
                    var secrets = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    if (secrets == null)
                    {
                        throw new InvalidOperationException("Couldn't parse the JSON file 'secrets.json'.");
                    }
                    return secrets[key];
                }
            }
        }
    }
}
