namespace Ldes.Test.WhenCallingTheLdesFeed;

using JsonLD.Core;
using Newtonsoft.Json.Linq;

public class TheResultingJsonLdDocument : IClassFixture<Setup>
{
    private readonly JObject? _document;

    public TheResultingJsonLdDocument()
    {
        var client = new JsonLdHttpClient(new Uri("http://localhost:8888"));

        var response = client.GetAsync("movies/1").GetAwaiter().GetResult();

        var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        _document = JObject.Parse(json);
    }

    [Fact]
    public void IsExpandable()
    {
        var expanded = JsonLdProcessor.Expand(_document);

        Assert.NotNull(expanded);
    }

    [Fact]
    public void IsParsableAsJsonLdWithFrame()
    {
        var context = JObject.Parse(File.ReadAllText("context.json"));

        var jObject = JsonLdProcessor.Frame(_document, context, new JsonLdOptions());

        Assert.NotNull(jObject);
    }
}
