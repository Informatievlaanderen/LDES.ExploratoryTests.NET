namespace AbbLdesReader.LdesModels;

public class LdesObject
{
    public string? Id { get; set; }
    public string[]? Type { get; set; }
    public string? Value { get; set; }
    public Dictionary<string, LdesProperty[]> Properties { get; set; } = new();

}
