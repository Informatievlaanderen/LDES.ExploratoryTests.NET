using System.Runtime.Serialization;
using JsonLD.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace Ldes.Test.WhenCallingTheRealLdesFeed;

public class TheResultingJsonLdDocument
{
    private readonly ITestOutputHelper _helper;
    public const string Feed = "https://dev.organisaties.abb.lblod.info/ldes/organizations/2";

    private readonly JArray _document;

    public TheResultingJsonLdDocument(ITestOutputHelper helper)
    {
        _helper = helper;
        var client = new JsonLdHttpClient(new Uri(Feed));

        var response = client.GetAsync("").GetAwaiter().GetResult();

        var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        _document = JArray.Parse(json);
    }

    [Fact]
    public async Task IsExpandable()
    {
        var expanded = JsonLdProcessor.Expand(_document);

        Assert.NotNull(expanded);
        var orgs = GetOrganisations(expanded);

        orgs.ForEach(x => _helper.WriteLine(JsonConvert.SerializeObject(x)));

        // next node

        var nextNode = expanded.Single(token =>
        {
            var jToken = (JObject)token;
            var hasType = jToken.TryGetValue("@type", out var type);

            return hasType && type.ToObject<List<string>>()
                .Contains("https://w3id.org/tree#GreaterThanOrEqualToRelation");
        }).ToObject<NextNode>();

        var client = new JsonLdHttpClient(new Uri(nextNode.Id.First().Value.Replace("organizations", "ldes/organizations")));

        var response = await client.GetAsync("");

        var json = await response.Content.ReadAsStringAsync();

        var nextNodeArray = JArray.Parse(json);

        var nextOrgs = GetOrganisations(nextNodeArray);

        nextOrgs.ForEach(x => _helper.WriteLine(JsonConvert.SerializeObject(x)));
    }

    private static List<Organisation> GetOrganisations(JArray ldesJson)
    {
        var orgs = ldesJson.Where(token =>
            {
                var jToken = (JObject)token;
                var hasType = jToken.TryGetValue("@type", out var type);

                return hasType && type.ToObject<List<string>>().Contains("http://www.w3.org/ns/org#Organization");
            })
            .Select(x => x.ToObject<Organisation>())
            .ToList();

        Assert.NotEmpty(orgs);
        return orgs;
    }

    [Fact]
    public void IsParsableAsJsonLdWithFrame()
    {
        var context = JObject.Parse(File.ReadAllText("context.json"));

        var jObject = JsonLdProcessor.Frame(_document, context, new JsonLdOptions());

        Assert.NotNull(jObject);
    }
}

[DataContract]
public class Organisation
{
    [DataMember(Name = "http://www.w3.org/ns/adms#identifier", IsRequired = false)]
    public LdesId[]? Id { get; set; }

    [DataMember(Name = "http://www.w3.org/2004/02/skos/core#prefLabel", IsRequired = false)]
    public LdesValue[]? PrefLabel { get; set; }
}

[DataContract]
public class LdesValue
{
    [DataMember(Name = "@value", IsRequired = false)]
    public string? Value { get; set; }
}

[DataContract]
public class LdesId
{
    [DataMember(Name = "@id", IsRequired = false)]
    public string? Value { get; set; }
}

[DataContract]
public class NextNode
{
    [DataMember(Name = "https://w3id.org/tree#node", IsRequired = false)]
    public LdesId[]? Id { get; set; }
}
