namespace LdesReader.Strategies;

public class OrganisationCache
{
    private readonly List<Organisation> _organisations = Array.Empty<Organisation>().ToList();
    private readonly List<ContactPoint> _contactPoints = Array.Empty<ContactPoint>().ToList();
    private readonly List<PrimarySite> _primarySites = Array.Empty<PrimarySite>().ToList();
    public string? LastEventId;

    public OrganisationCache(string? lastEventId)
    {
        LastEventId = lastEventId;
    }

    public void AddOrganisation(Organisation organisation)
    {
        Console.WriteLine($"\tOrganisatie '{organisation.Id}' toegevoegd!");

        _organisations.RemoveAll(o => organisation.Id == o.Id);
        _organisations.Add(organisation);
        LinkOrganisationWithPrimarySite(organisation.Site?.Id);
    }


    public void AddContactPoint(ContactPoint contactPoint)
    {
        Console.WriteLine($"\tContact Point '{contactPoint.Id}' toegevoegd!");

        _contactPoints.RemoveAll(c => c.Id == contactPoint.Id);
        _contactPoints.Add(contactPoint);

        LinkContactPointWithPrimarySite(contactPoint.Id);
    }

    public void AddPrimarySite(PrimarySite primarySite)
    {
        Console.WriteLine($"\tPrimary Site '{primarySite.Id}' toegevoegd!");

        _primarySites.RemoveAll(p => p.Id == primarySite.Id);
        _primarySites.Add(primarySite);

        LinkOrganisationWithPrimarySite(primarySite.Id);
        LinkOrganisationWithPrimarySite(primarySite.Address?.Id);
    }

    public IEnumerable<Organisation> GetOrganisations() => _organisations;

    private void LinkOrganisationWithPrimarySite(string? siteId)
    {
        if (siteId is null) return;

        var organisation = _organisations.SingleOrDefault(o => o.Site?.Id == siteId);
        var primarySite = _primarySites.SingleOrDefault(p => p.Id == siteId);

        if (primarySite is null) return;
        if (organisation is null) return;

        Console.WriteLine(
            $"\t\tOrganisatie '{organisation.Id}' kreeg address {primarySite.Id}");
        organisation.Site = primarySite;
    }

    private void LinkContactPointWithPrimarySite(string? contactPointId)
    {
        if (contactPointId is null) return;

        var contactPoint = _contactPoints.SingleOrDefault(o => o.Id == contactPointId);
        var primarySite = _primarySites.SingleOrDefault(p => p.Address?.Id == contactPointId);

        if (primarySite is null) return;
        if (contactPoint is null) return;

        Console.WriteLine($"\t\tContact point '{contactPoint.Id}' toegekend aan address {primarySite.Id}");
        primarySite.Address = contactPoint;
    }
}
