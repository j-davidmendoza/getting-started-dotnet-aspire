using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;
using RatingService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.AddServiceDefaults();

builder.AddRedisClient(connectionName: "cache");

builder.Services.AddSingleton(TracerProvider.Default.GetTracer(builder.Environment.ApplicationName));
builder.Services.AddSingleton<RatingStore>();

var app = builder.Build();

app.UseCors(x => x.AllowAnyOrigin());

app.MapGet("/ratings", async ([FromQuery] string podcastName, RatingStore ratingStore) =>
{
    if (string.IsNullOrWhiteSpace(podcastName))
        return Results.BadRequest("Please provide a podcastName on the query string.");

    var rating = await ratingStore.GetAverageRating(podcastName);

    return Results.Ok(rating);
});

app.MapPost("/ratings", ([FromBody] SubmitRatingRequestBody body, RatingStore ratingStore) =>
{
    if (string.IsNullOrWhiteSpace(body.PodcastName))
        return Results.BadRequest("Please provide a podcastName on the query string.");

    ratingStore.AddRating(body.PodcastName, body.Rating);

    return Results.Ok();
});

app.MapDefaultEndpoints();

app.Run();