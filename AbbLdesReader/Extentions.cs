using Newtonsoft.Json.Linq;

namespace AbbLdesReader;

public static class Extentions
{
    public static JToken? GetOrDefault(this JObject token, string propertyName) =>
        token.TryGetValue(propertyName, out var value) ? value : null;
}
