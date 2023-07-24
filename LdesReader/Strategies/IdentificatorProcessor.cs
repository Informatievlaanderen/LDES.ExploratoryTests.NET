using AbbLdesReader.LdesModels;
using LdesReader.Models;
using LdesReader.Models.Ldes;

namespace LdesReader.Strategies;

public class IdentificatorProcessor : IsVersionOfEventProcessor
{
    private readonly OrganisationCache _cache;
    protected override IEnumerable<string> VersionsToProcess => new[] { "http://data.lblod.info/id/identificatoren/" };

    public IdentificatorProcessor(OrganisationCache cache)
    {
        _cache = cache;
    }

    public override Task ProcessAsync(LdesObject ldesObject)
    {
        var ldesContactPoint = new LdesIdentificator(ldesObject);
        var identificator = GetIdentificator(ldesContactPoint);

        _cache.AddIdentificator(identificator);

        return Task.CompletedTask;
    }

    private Identificator GetIdentificator(LdesIdentificator ldesIdentificator) =>
        new()
        {
            Id = ldesIdentificator.IsVersionOf,
            Type = ldesIdentificator.Notation!,
            Value = new IdentificatorValue()
            {
                Id = ldesIdentificator.GestructureerdeIdentificator,
            }
        };
}
