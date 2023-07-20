using AbbLdesReader;
using AbbLdesReader.LdesModels;
using Newtonsoft.Json;

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

        if (organisation.Ovonummer is null && organisation.Kbonummer is null) return;

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

        var tripleClient = new HttpTripleClient();

        foreach (var idValue in ldesOrganisation.Identities ?? Array.Empty<string?>())
        {
            var directTriples = await tripleClient.GetTriplesForSubject(idValue);
            var idTypeTriple = directTriples.GetTripleByPredicate("http://www.w3.org/2004/02/skos/core#notation");

            if (idTypeTriple?.Object.Value == "OVO-nummer")
                organisation.Ovonummer ??= await GetIdentificatorValue(directTriples);
            if (idTypeTriple?.Object.Value == "KBO nummer")
                organisation.Kbonummer ??= await GetIdentificatorValue(directTriples);
        }

        organisation.Voorkeursnaam = ldesOrganisation.PrefLabel;

        var primarySiteId = ldesOrganisation.PrimarySite;
        if (primarySiteId is not null)
            organisation.Site = new PrimarySite
            {
                Id = primarySiteId,
            };

        return organisation;
    }

    private static async Task<string?> GetIdentificatorValue(DirectTriples directTriples)
    {
        var tripleClient = new HttpTripleClient();
        var gestructureerdeIdentificator = directTriples.GetTripleByPredicate(
            "https://data.vlaanderen.be/ns/generiek#gestructureerdeIdentificator");

        var idlinkDirectTriples =
            await tripleClient.GetTriplesForSubject(gestructureerdeIdentificator?.Object.Value);

        var ovoTriple = idlinkDirectTriples.GetTripleByPredicate(
            "https://data.vlaanderen.be/ns/generiek#lokaleIdentificator");
        return ovoTriple?.Object.Value;
    }

    private static async Task<DirectTriples> GetTriples(string? idValue)
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri("https://data.lblod.info/uri-info/direct"),
        };
        var directIdResponse = await client.GetAsync($"?subject={idValue}");
        var directIdContent = await directIdResponse.Content.ReadAsStringAsync();
        var directTriples = JsonConvert.DeserializeObject<DirectTriples>(directIdContent);
        return directTriples;
    }
}
