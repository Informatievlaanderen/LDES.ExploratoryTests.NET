using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AbbLdesReader.LdesModels;

public class LdesObjectConverter : JsonConverter
{
    public override bool CanRead => true;
    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jArray = JArray.Load(reader);
        return ToLdesArray(jArray);
    }

    private IEnumerable<LdesObject> ToLdesArray(JToken jToken) => jToken.Select(ToLdesObject);

    private LdesObject ToLdesObject(JToken jToken)
    {
        var jObject = (JObject)jToken;
        var ldesObject = new LdesObject
        {
            Id = jObject.GetOrDefault("@id")?.Value<string>(),
            Type = jObject.GetOrDefault("@type")?.Values<string>().ToArray(),
            Value = jObject.GetOrDefault("@value")?.Value<string>(),
            Properties = jObject
                .Properties()
                .Where(p => p.Name is not ("@id" or "@type" or "@value"))
                .ToDictionary(p => p.Name, p => p.Value.Select(ToLdesProperty).ToArray()),
        };
        return ldesObject;
    }

    private LdesProperty ToLdesProperty(JToken jToken)
    {
        var jObject = (JObject)jToken;
        var ldesObject = new LdesProperty
        {
            Id = jObject.GetOrDefault("@id")?.Value<string>(),
            Type = jObject.GetOrDefault("@type")?.Value<string>(),
            Value = jObject.GetOrDefault("@value")?.Value<string>(),
        };
        return ldesObject;
    }

    public override bool CanConvert(Type objectType) => typeof(IEnumerable<LdesObject>).IsAssignableFrom(objectType);
}
