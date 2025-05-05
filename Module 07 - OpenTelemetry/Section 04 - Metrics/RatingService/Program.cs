using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;
using RatingService;
using System.Runtime.CompilerServices;

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

    //This is a static property called curreny span
    Tracer.CurrentSpan.SetAttribute("PodcastName", podcastName);
    Tracer.CurrentSpan.SetAttribute("Rating", rating);

    return Results.Ok(rating);
});

app.MapPost("/ratings", ([FromBody] SubmitRatingRequestBody body, RatingStore ratingStore) =>
{
    if (string.IsNullOrWhiteSpace(body.PodcastName))
        return Results.BadRequest("Please provide a podcastName on the query string.");

    //if in a span you have info, but you dont care when when it happens use an attirbute
    //a span has a lifetime, within that span if you want to know when it happens within that span, use an event
    //Can add attributes to a span at any time
    Tracer.CurrentSpan.AddEvent("Something has happened.");

    ratingStore.AddRating(body.PodcastName, body.Rating);

    Tracer.CurrentSpan.SetAttribute("PodcastName", body.PodcastName);
    Tracer.CurrentSpan.SetAttribute("Rating", body.Rating);

    return Results.Ok();
});

app.MapDefaultEndpoints();

app.Run();