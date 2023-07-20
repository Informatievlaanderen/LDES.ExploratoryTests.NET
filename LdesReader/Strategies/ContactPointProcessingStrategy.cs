using AbbLdesReader.LdesModels;

namespace LdesReader.Strategies;

public class ContactPointProcessingStrategy : IsVersionOfEventProcessor
{
    private readonly OrganisationCache _cache;
    private const string ContactPuntVersion = "http://data.lblod.info/id/contact-punten/";

    protected override IEnumerable<string> VersionsToProcess => new[] { ContactPuntVersion };

    public ContactPointProcessingStrategy(OrganisationCache cache)
    {
        _cache = cache;
    }

    public override Task ProcessAsync(LdesObject ldesObject)
    {
        var ldesContactPoint = new LdesContactPoint(ldesObject);
        var contactPoint = GetContactPoint(ldesContactPoint);

        _cache.AddContactPoint(contactPoint);

        return Task.CompletedTask;
    }

    private static ContactPoint GetContactPoint(LdesContactPoint ldesContactPoint)
    {
        var contactPoint = new ContactPoint
        {
            Id = ldesContactPoint.IsVersionOf!,
            EventId = ldesContactPoint.LdesObject.Id!,
            EventTimestamp = ldesContactPoint.GeneratedAtTime!,
        };


        contactPoint.Email = ldesContactPoint.Email;
        contactPoint.Telephone = ldesContactPoint.Telephone;
        contactPoint.Website = ldesContactPoint.Webpage;
        contactPoint.ContactType = ldesContactPoint.ContactType;
        contactPoint.UUID = ldesContactPoint.Uuid;

        return contactPoint;
    }
}
