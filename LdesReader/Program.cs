using System.Globalization;
using AbbLdesReader;
using AbbLdesReader.LdesModels;
using CsvHelper;
using LdesReader;
using LdesReader.Models;
using LdesReader.Strategies;

const string feedUri = "https://dev.organisaties.abb.lblod.info/ldes/organizations/";
var pageNr = 1;

var ldesProcessor = new LdesProcessor();
//var lastEventId = "http://mu.semte.ch/services/ldes-time-fragmenter/versioned/93132334-fdeb-42ba-b7e8-05a9390c6569";
var lastEventId = (string?)null;
var cache = new OrganisationCache(lastEventId);

await ldesProcessor
    .NextPageBy(GetNextPage)
    .WithEventProcessor(new OrganisationProcessingStrategy(cache))
    .WithEventProcessor(new PrimarySiteProcessingStrategy(cache))
    .WithEventProcessor(new ContactPointProcessingStrategy(cache))
    .WithEventProcessor(new IdentificatorProcessor(cache))
    .WithEventProcessor(new GestructureerdeIdentificatorProcessor(cache))
    .WithEventProcessor(new EventEventProcessor(cache))
    .Start($"{feedUri}{pageNr}", cache.LastEventId);

Console.WriteLine("Schrijven naar CSV...");

await using (var writer = new StreamWriter("organisations.csv"))
await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
{
    var organisations = MapToCsvValues(cache.GetOrganisations().ToArray());
    csv.WriteRecords(organisations);
}

Console.WriteLine($"En klaar tot {cache.LastEventId}!");

static string? GetNextPage(IEnumerable<LdesObject> ldesArray)
{
    var nextNode = ldesArray.SingleOrDefault(ldesObject =>
        ldesObject.Type?.Contains("https://w3id.org/tree#GreaterThanOrEqualToRelation") ?? false);

    if (nextNode is null) return null;
    return nextNode.Properties.TryGetValue("https://w3id.org/tree#node", out var nextUri)
        ? nextUri[0].Id
        : null;
}

static CsvOrganisation[] MapToCsvValues(Organisation[] organisations) =>
    organisations.Select(o =>
    {
        return new CsvOrganisation
        {
            OvoNummer = o.Identities?.SingleOrDefault(i => new string(i.Type.ToLower().Where(char.IsLetter).ToArray()) == "ovonummer")?.Value.Value,
            KboNummer = o.Identities?.SingleOrDefault(i => new string(i.Type.ToLower().Where(char.IsLetter).ToArray()) == "kbonummer")?.Value.Value,
            PrefLabel = o.Voorkeursnaam,
            Primair = ToContactInfo(o.Site?.ContactPoints?.SingleOrDefault(c => c.ContactType?.ToLower() == "primary")),
            Secundair = ToContactInfo(o.Site?.ContactPoints?.SingleOrDefault(c => c.ContactType?.ToLower() == "secondary")),
        };

        static CsvOrganisation.ContactInfo? ToContactInfo(ContactPoint? contactPoint)
        {
            if(contactPoint is null) return null;
            return new CsvOrganisation.ContactInfo
            {
                Email = contactPoint.Email,
                Website = contactPoint.Website,
                Telefoon = contactPoint.Telephone,
            };
        }
    }).ToArray();
