namespace Ldes.Test;

public class JsonLdHttpClient : HttpClient
{
    public JsonLdHttpClient(Uri baseAddress)
    {
        BaseAddress = baseAddress;
        DefaultRequestHeaders.Add("Accept", "application/ld+json");
    }
}
