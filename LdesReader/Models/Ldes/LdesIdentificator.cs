using AbbLdesReader.LdesModels;

namespace LdesReader.Models.Ldes;

public class LdesIdentificator
{
    public LdesObject LdesObject { get; }

    public LdesIdentificator(LdesObject ldesObject)
    {
        LdesObject = ldesObject;
    }

    public string? IsVersionOf =>
        LdesObject.Properties.TryGetValue("http://purl.org/dc/terms/isVersionOf", out var value) ? value[0].Id : null;

    public string? GeneratedAtTime =>
        LdesObject.Properties.TryGetValue("http://www.w3.org/ns/prov#generatedAtTime", out var value)
            ? value[0].Value
            : null;

    public string? Notation =>
        LdesObject.Properties.TryGetValue("http://www.w3.org/2004/02/skos/core#notation", out var value)
            ? value[0].Value
            : null;

    public string? GestructureerdeIdentificator =>
        LdesObject.Properties.TryGetValue("https://data.vlaanderen.be/ns/generiek#gestructureerdeIdentificator",
            out var value)
            ? value[0].Id
            : null;
}
