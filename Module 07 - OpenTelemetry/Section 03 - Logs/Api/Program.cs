using Api;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.AddServiceDefaults();

builder.Services.AddHttpClient<RatingServiceHttpClient>(x => x.BaseAddress = new Uri("https+http://ratingservice"));

builder.Services.AddSingleton(TracerProvider.Default.GetTracer(builder.Environment.ApplicationName));

builder.AddSqlServerDbContext<PodcastDbContext>(connectionName: "podcasts");

var app = builder.Build();

app.UseCors(x => x.AllowAnyOrigin());

app.MapGet("/podcasts", async (PodcastDbContext db, RatingServiceHttpClient ratingServiceHttpClient) =>
{
    var all = await db.Podcasts
        .OrderBy(x => x.Title)
        .ToListAsync();

    var withRatings = new List<PodcastApiModel>();

    foreach (var podcast in all)
    {
        withRatings.Add(new PodcastApiModel(
            podcast.Title,
            await ratingServiceHttpClient.GetRating(podcast.Title)
        ));
    }

    return withRatings;
});

app.MapPost("/rating", async (RatingServiceHttpClient ratingServiceHttpClient, [FromBody] PodcastApiModel podcast) =>
{
    await ratingServiceHttpClient.SubmitRating(podcast.PodcastName, podcast.Rating);
});

app.MapDefaultEndpoints();

app.Run();

record PodcastApiModel(string PodcastName, int Rating);
