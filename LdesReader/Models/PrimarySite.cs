namespace LdesReader;

public class PrimarySite
{
    public string Id { get; set; }
    public ContactPoint? Address { get; set; }
    public int Page { get; set; }
    public string EventId { get; set; }
    public string EventTimestamp { get; set; }
}
