using System.Globalization;
using AbbLdesReader;
using AbbLdesReader.LdesModels;
using CsvHelper;
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
    .WithEventProcessor(new EventEventProcessor(cache))
    .Start($"{feedUri}{pageNr}", cache.LastEventId);

Console.WriteLine("Schrijven naar CSV...");

await using (var writer = new StreamWriter("organisations.csv"))
await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
{
    csv.WriteRecords(cache.GetOrganisations());
}

Console.WriteLine($"En klaar tot {cache.LastEventId}!");

static string? GetNextPage(IEnumerable<LdesObject> ldesArray)
{
    var nextNode = ldesArray.SingleOrDefault(ldesObject =>
        ldesObject.Type?.Contains("https://w3id.org/tree#GreaterThanOrEqualToRelation") ?? false);

    if (nextNode is null) return null;
    return nextNode.Properties.TryGetValue("https://w3id.org/tree#node", out var nextUri)
        ? nextUri[0].Id?.Replace("/organizations/", "/ldes/organizations/")
        : null;
}
