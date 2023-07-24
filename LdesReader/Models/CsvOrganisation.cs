namespace LdesReader.Models;

public class CsvOrganisation
{
    public string? OvoNummer { get; set; }
    public string? KboNummer { get; set; }
    public string? PrefLabel { get; set; }
    public ContactInfo? Primair { get; set; }
    public ContactInfo? Secundair { get; set; }

    public class ContactInfo
    {
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? Telefoon { get; set; }
    }
}
