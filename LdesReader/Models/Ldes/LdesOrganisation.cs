using System.Runtime.Serialization;
using AbbLdesReader.LdesModels;

namespace LdesReader.Models.Ldes;

[DataContract]
public class LdesOrganisation
{
    public LdesObject LdesObject { get; }

    public LdesOrganisation(LdesObject ldesObject)
    {
        LdesObject = ldesObject;
    }

    public string[]? Identities =>
        LdesObject.Properties.TryGetValue("http://www.w3.org/ns/adms#identifier", out var value)
            ? value.Select(v => v.Id!).ToArray()
            : null;

    public string? PrefLabel =>
        LdesObject.Properties.TryGetValue("http://www.w3.org/2004/02/skos/core#prefLabel", out var value)
            ? value[0].Value
            : null;

    public string? PrimarySite =>
        LdesObject.Properties.TryGetValue("http://www.w3.org/ns/org#hasPrimarySite", out var value)
            ? value[0].Id
            : null;

    public string? GeneratedAtTime =>
        LdesObject.Properties.TryGetValue("http://www.w3.org/ns/prov#generatedAtTime", out var value)
            ? value[0].Value
            : null;

    public string? IsVersionOf =>
        LdesObject.Properties.TryGetValue("http://purl.org/dc/terms/isVersionOf", out var value)
            ? value[0].Id
            : null;
}
