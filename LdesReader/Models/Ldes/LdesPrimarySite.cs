using System.Runtime.Serialization;
using AbbLdesReader.LdesModels;

namespace LdesReader;

[DataContract]
public class LdesPrimarySite
{
    public LdesObject LdesObject { get; }

    public LdesPrimarySite(LdesObject ldesObject)
    {
        LdesObject = ldesObject;
    }

    public string? IsVersionOf =>
        LdesObject.Properties.TryGetValue("http://purl.org/dc/terms/isVersionOf", out var value)
            ? value[0].Id
            : null;

    public string? SiteAddress =>
        LdesObject.Properties.TryGetValue("http://www.w3.org/ns/org#siteAddress", out var value)
            ? value[0].Id
            : null;

    public string? GeneratedAtTime =>
        LdesObject.Properties.TryGetValue("http://www.w3.org/ns/prov#generatedAtTime", out var value)
            ? value[0].Value
            : null;
}
