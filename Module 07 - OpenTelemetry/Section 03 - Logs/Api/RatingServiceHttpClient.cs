namespace Api;

public class RatingServiceHttpClient(HttpClient httpClient, ILogger<RatingServiceHttpClient> logger) : HttpClient
{
    public Task<int> GetRating(string podcastName) =>
        httpClient.GetFromJsonAsync<int>($"/ratings?podcastName={podcastName}");

    public Task SubmitRating(string podcastName, int rating)
    {
        logger.LogInformation("Submit rating called for podcast {podcastName} with rating {rating}",
            podcastName, rating);

        return httpClient.PostAsJsonAsync($"/ratings",
            new
            {
                PodcastName = podcastName,
                Rating = rating,
            });
    }
}
