using AbbLdesReader;
using AbbLdesReader.LdesModels;
using LdesReader.Models;
using LdesReader.Models.Ldes;
using Newtonsoft.Json.Linq;

namespace LdesReader.Strategies;

public class PrimarySiteProcessingStrategy : IsVersionOfEventProcessor
{
    private readonly OrganisationCache _cache;
    private const string VestigingVersion = "http://data.lblod.info/id/vestigingen/";

    protected override IEnumerable<string> VersionsToProcess => new[] { VestigingVersion };

    public PrimarySiteProcessingStrategy(OrganisationCache cache)
    {
        _cache = cache;
    }

    public override Task ProcessAsync(LdesObject ldesObject)
    {
        var ldesPrimarySite = new LdesPrimarySite(ldesObject);
        var primarySite = GetPrimarySite(ldesPrimarySite!);

        _cache.AddPrimarySite(primarySite);
        return Task.CompletedTask;
    }

    private static PrimarySite GetPrimarySite(LdesPrimarySite ldesPrimarySite)
    {
        var primarySite = new PrimarySite
        {
            Id = ldesPrimarySite.IsVersionOf!,
            EventId = ldesPrimarySite.LdesObject.Id!,
            EventTimestamp = ldesPrimarySite.GeneratedAtTime!,
        };

        var siteAddressId = ldesPrimarySite.SiteAddress;
        if (siteAddressId is not null)
            primarySite.ContactPoints = siteAddressId.Select(id => new ContactPoint
            {
                Id = id,
            }).ToArray();
        return primarySite;
    }
}
