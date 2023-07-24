namespace LdesReader.Models;

public class PrimarySite
{
    public string Id { get; set; }
    public ContactPoint[]? ContactPoints { get; set; }
    public string EventId { get; set; }
    public string EventTimestamp { get; set; }
}
