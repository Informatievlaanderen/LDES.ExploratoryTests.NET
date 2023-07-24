using LdesReader.Models;
using LdesReader.Models.Ldes;

namespace LdesReader;

public class OrganisationCache
{
    private readonly List<Organisation> _organisations = Array.Empty<Organisation>().ToList();
    private readonly List<ContactPoint> _contactPoints = Array.Empty<ContactPoint>().ToList();
    private readonly List<PrimarySite> _primarySites = Array.Empty<PrimarySite>().ToList();
    private readonly List<Identificator> _identificators = Array.Empty<Identificator>().ToList();
    private readonly List<IdentificatorValue> _identificatorValues = Array.Empty<IdentificatorValue>().ToList();

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
        if (organisation.Identities == null) return;
        foreach (var identity in organisation.Identities)
        {
            LinkOrganisationWithIdentificator(identity.Id);
        }
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
        if (primarySite.ContactPoints == null) return;
        foreach (var contactPoint in primarySite.ContactPoints)
        {
            LinkContactPointWithPrimarySite(contactPoint.Id);
        }
    }

    public void AddIdentificator(Identificator identificator)
    {
        Console.WriteLine($"\tIdentificator '{identificator.Id}' toegevoegd!");

        _identificators.RemoveAll(c => c.Id == identificator.Id);
        _identificators.Add(identificator);

        LinkOrganisationWithIdentificator(identificator.Id);
        LinkIdentificatorWithValue(identificator.Value.Id);
    }

    public void AddIdentificatorValue(IdentificatorValue identificatorValue)
    {
        Console.WriteLine($"\tIdentificator Value '{identificatorValue.Id}' toegevoegd!");

        _identificatorValues.RemoveAll(c => c.Id == identificatorValue.Id);
        _identificatorValues.Add(identificatorValue);

        LinkIdentificatorWithValue(identificatorValue.Id);
    }

    public IEnumerable<Organisation> GetOrganisations() => _organisations;

    private void LinkOrganisationWithIdentificator(string? identificatorId)
    {
        if (identificatorId is null) return;

        var organisation =
            _organisations.SingleOrDefault(o => o.Identities?.Any(i => i.Id == identificatorId) ?? false);
        var identificator = _identificators.SingleOrDefault(p => p.Id == identificatorId);

        if (identificator is null) return;
        if (organisation is null) return;

        Console.WriteLine(
            $"\t\tOrganisatie '{organisation.Id}' kreeg identity {identificator.Id}");
        organisation.Identities = organisation.Identities!.Where(i => i.Id != identificator.Id).Append(identificator)
            .ToArray();
    }

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
        var primarySite =
            _primarySites.SingleOrDefault(p => p.ContactPoints?.Any(c => c.Id == contactPointId) ?? false);

        if (primarySite is null) return;
        if (contactPoint is null) return;

        Console.WriteLine($"\t\tContact point '{contactPoint.Id}' toegekend aan address {primarySite.Id}");
        primarySite.ContactPoints = primarySite.ContactPoints!.Where(c => c.Id != contactPointId).Append(contactPoint)
            .ToArray();
    }

    private void LinkIdentificatorWithValue(string? identificatorValueId)
    {
        if (identificatorValueId is null) return;

        var identificator = _identificators.SingleOrDefault(i => i.Value.Id == identificatorValueId);
        var identificatorValue = _identificatorValues.SingleOrDefault(iv => iv.Id == identificatorValueId);

        if (identificatorValue is null) return;
        if (identificator is null) return;

        Console.WriteLine($"\t\tIdentifier '{identificator.Id}' kreeg value {identificatorValue.Id}");
        identificator.Value = identificatorValue;
    }
}
