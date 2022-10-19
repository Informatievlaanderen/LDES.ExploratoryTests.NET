namespace Ldes.Test;

using System.Net.Http.Headers;

public class Setup
{
    public Setup()
    {
        var client = new HttpClient();
        var identifier = 800005;
        Enumerable.Range(80000, 20)
            .ToList()
            .ForEach(i => PostMovie(client, i.ToString()));
    }

    private static void PostMovie(HttpClient client, string identifier)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri =
                new Uri($"http://localhost:8888/movies?resource=https%3A%2F%2Fexample.org%2Fmovies%2F{identifier}"),
            Content = new StringContent(
                $"<https://example.org/movies/{identifier}> a <https://example.org/Movie>;\n    <https://example.org/name> \"My new movie\";\n    <https://example.org/genres> \"Comedy|Romance|SciFi\".")
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue("text/turtle")
                }
            }
        };
        using var response = client.SendAsync(request).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
    }
}
