using AbbLdesReader.LdesModels;
using Newtonsoft.Json;

namespace AbbLdesReader;

public class LdesReader
{
    public static async Task<IEnumerable<LdesObject>> ReadPage(Uri ldesUri)
    {
        var ldesClient = new HttpClient
        {
            DefaultRequestHeaders = { { "Accept", "application/ld+json" } },
        };
        var response = await ldesClient.GetAsync(ldesUri);
        var json = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<IEnumerable<LdesObject>>(json, new LdesObjectConverter());
    }
}
