using AbbLdesReader.LdesModels;

namespace LdesReader.Models.Ldes;

public class LdesGestructureerdeIdentifier
{
    public LdesObject LdesObject { get; }

    public LdesGestructureerdeIdentifier(LdesObject ldesObject)
    {
        LdesObject = ldesObject;
    }

    public string? IsVersionOf =>
        LdesObject.Properties.TryGetValue("http://purl.org/dc/terms/isVersionOf", out var value) ? value[0].Id : null;

    public string? GeneratedAtTime =>
        LdesObject.Properties.TryGetValue("http://www.w3.org/ns/prov#generatedAtTime", out var value)
            ? value[0].Value
            : null;

    public string? LokaleIdentificator =>
        LdesObject.Properties.TryGetValue("https://data.vlaanderen.be/ns/generiek#lokaleIdentificator", out var value)
            ? value[0].Value
            : null;
}
