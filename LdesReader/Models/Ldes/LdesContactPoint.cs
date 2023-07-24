using AbbLdesReader.LdesModels;

namespace LdesReader.Models.Ldes;

public class LdesContactPoint
{
    public LdesObject LdesObject { get; }

    public LdesContactPoint(LdesObject ldesObject)
    {
        LdesObject = ldesObject;
    }

    public string? IsVersionOf =>
        LdesObject.Properties.TryGetValue("http://purl.org/dc/terms/isVersionOf", out var value) ? value[0].Id : null;

    public string? GeneratedAtTime =>
        LdesObject.Properties.TryGetValue("http://www.w3.org/ns/prov#generatedAtTime", out var value)
            ? value[0].Value
            : null;

    public string? ContactType =>
        LdesObject.Properties.TryGetValue("http://schema.org/contactType", out var value) ? value[0].Value : null;

    public string? Uuid =>
        LdesObject.Properties.TryGetValue("http://mu.semte.ch/vocabularies/core/uuid", out var value)
            ? value[0].Value
            : null;

    public string? Telephone =>
        LdesObject.Properties.TryGetValue("http://schema.org/telephone", out var value) ? value[0].Value : null;

    public string? Webpage =>
        LdesObject.Properties.TryGetValue("http://xmlns.com/foaf/0.1/page", out var value) ? value[0].Value : null;

    public string? Email =>
        LdesObject.Properties.TryGetValue("http://schema.org/email", out var value) ? value[0].Value : null;
}
