using Newtonsoft.Json;

namespace LdesReader;

public class HttpTripleClient
{
    public async Task<DirectTriples> GetTriplesForSubject(string? subject)
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri("https://data.lblod.info/uri-info/direct"),
        };
        var directIdResponse = await client.GetAsync($"?subject={subject}");
        var directIdContent = await directIdResponse.Content.ReadAsStringAsync();
        var directTriples = JsonConvert.DeserializeObject<DirectTriples>(directIdContent);
        return directTriples;
    }
}
