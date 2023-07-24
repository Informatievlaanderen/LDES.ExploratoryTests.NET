using AbbLdesReader;
using AbbLdesReader.LdesModels;
using LdesReader.Models;
using LdesReader.Models.Ldes;

namespace LdesReader.Strategies;

public class OrganisationProcessingStrategy : IsVersionOfEventProcessor
{
    private const string OrganisationKboVersion = "http://data.lblod.info/id/besturenVanDeEredienst/";
    private const string OrganisationVersion = "http://data.lblod.info/id/bestuurseenheden/";

    private readonly OrganisationCache _cache;

    protected override IEnumerable<string> VersionsToProcess => new[] { OrganisationKboVersion, OrganisationVersion };

    public OrganisationProcessingStrategy(OrganisationCache cache)
    {
        _cache = cache;
    }

    public override async Task ProcessAsync(LdesObject ldesObject)
    {
        var ldesOrganisation = new LdesOrganisation(ldesObject);
        var organisation = await GetOrganisation(ldesOrganisation);

        _cache.AddOrganisation(organisation);
    }

    private static async Task<Organisation> GetOrganisation(LdesOrganisation ldesOrganisation)
    {
        var organisation = new Organisation
        {
            Id = ldesOrganisation.IsVersionOf!,
            EventId = ldesOrganisation.LdesObject.Id!,
            EventTimestamp = ldesOrganisation.GeneratedAtTime!,
        };


        organisation.Identities = ldesOrganisation.Identities?.Select(i => new Identificator()
        {
            Id = i,
        }).ToArray();

        organisation.Voorkeursnaam = ldesOrganisation.PrefLabel;

        var primarySiteId = ldesOrganisation.PrimarySite;
        if (primarySiteId is not null)
            organisation.Site = new PrimarySite
            {
                Id = primarySiteId,
            };

        return organisation;
    }
}
