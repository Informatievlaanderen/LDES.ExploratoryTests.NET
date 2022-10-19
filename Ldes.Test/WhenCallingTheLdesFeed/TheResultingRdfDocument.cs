namespace Ldes.Test.WhenCallingTheLdesFeed;

using JsonLD.Core;
using Newtonsoft.Json.Linq;

public class TheResultingRdfDocument : IClassFixture<Setup>
{
    private readonly JObject? _document;
    private readonly RDFDataset _rdf;

    public TheResultingRdfDocument()
    {
        var client = new JsonLdHttpClient(new Uri("http://localhost:8888"));

        var response = client.GetAsync("movies/3").GetAwaiter().GetResult();

        var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        _document = JObject.Parse(json);
        _rdf = (RDFDataset)JsonLdProcessor.ToRDF(_document);
    }

    [Fact]
    public void IsNotNull()
    {
        Assert.NotNull(_rdf);
    }

    [Fact]
    public void CanBeTransformedToDictionary()
    {
        var dictionary = ToDictionary();

        Assert.True(dictionary.Any());

        foreach (var movie in dictionary) Assert.True(movie.Value.Any());
    }

    private Dictionary<string, Dictionary<string, string>> ToDictionary()
    {
        var quads = _rdf.GetQuads("@default");

        var ids = quads
            .Where(quad => quad.GetObject().GetValue() == "https://example.org/Movie")
            .Select(quad => new { value = quad.GetSubject().GetValue() })
            .ToList();

        var movies = new Dictionary<string, Dictionary<string, string>>();

        foreach (var id in ids)
        {
            var quadsDictionary = quads
                .Where(quad => quad.GetSubject().GetValue() == id.value)
                .Select(quad => new { topic = quad.GetPredicate().GetValue(), value = quad.GetObject().GetValue() })
                .Distinct()
                .ToDictionary(x => x.topic, x => x.value);

            movies.Add(id.value, quadsDictionary);
        }

        return movies
            .OrderBy(x => x.Value["http://www.w3.org/ns/prov#generatedAtTime"])
            .ToDictionary(x => x.Key, x => x.Value);
    }
}
