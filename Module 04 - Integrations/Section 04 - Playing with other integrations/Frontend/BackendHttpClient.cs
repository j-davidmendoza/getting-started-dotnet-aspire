namespace Frontend;

public class BackendHttpClient(HttpClient httpClient) : HttpClient
{
    public Task<List<string>?> GetPodcasts() =>
        httpClient.GetFromJsonAsync<List<string>>("/podcasts");
}
