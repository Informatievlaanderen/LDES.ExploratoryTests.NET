namespace LdesReader.Models;

public class Organisation
{
    public string Id { get; set; }
    public string? Voorkeursnaam { get; set; }
    public PrimarySite? Site { get; set; }
    public string EventId { get; set; }
    public string EventTimestamp { get; set; }
    public Identificator[]? Identities { get; set; }
}
