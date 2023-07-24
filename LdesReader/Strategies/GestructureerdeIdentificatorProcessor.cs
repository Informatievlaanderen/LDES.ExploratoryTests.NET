using AbbLdesReader.LdesModels;
using LdesReader.Models;
using LdesReader.Models.Ldes;

namespace LdesReader.Strategies;

public class GestructureerdeIdentificatorProcessor : IsVersionOfEventProcessor
{
    private readonly OrganisationCache _cache;

    protected override IEnumerable<string> VersionsToProcess =>
        new[] { "http://data.lblod.info/id/gestructureerdeIdentificatoren/" };

    public GestructureerdeIdentificatorProcessor(OrganisationCache cache)
    {
        _cache = cache;
    }

    public override Task ProcessAsync(LdesObject ldesObject)
    {
        var ldesGestructureerdeIdentifier = new LdesGestructureerdeIdentifier(ldesObject);
        var identificator = GetGestructureerdeIdentificator(ldesGestructureerdeIdentifier);

        _cache.AddIdentificatorValue(identificator);

        return Task.CompletedTask;
    }

    private static IdentificatorValue GetGestructureerdeIdentificator(LdesGestructureerdeIdentifier ldesGestructureerdeIdentifier) =>
        new()
        {
            Id = ldesGestructureerdeIdentifier.IsVersionOf,
            Value = ldesGestructureerdeIdentifier.LokaleIdentificator,
        };
}
