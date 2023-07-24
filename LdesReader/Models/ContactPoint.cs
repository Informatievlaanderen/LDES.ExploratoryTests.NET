namespace LdesReader.Models;

public class ContactPoint
{
    public string Id { get; set; }
    public string? UUID { get; set; }
    public string? ContactType { get; set; }
    public string? Website { get; set; }
    public string? Telephone { get; set; }
    public string? Email { get; set; }
    public string EventId { get; set; }
    public string EventTimestamp { get; set; }
}
