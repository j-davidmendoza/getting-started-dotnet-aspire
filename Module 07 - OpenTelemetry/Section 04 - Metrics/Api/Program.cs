using System.Diagnostics.Metrics;
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

builder.AddSqlServerDbContext<PodcastDbContext>(connectionName: "podcasts",
    x => x.DisableTracing = true);

builder.Services.AddOpenTelemetry().WithTracing(TracerProviderBuilder =>
    {
        TracerProviderBuilder.AddSqlClientInstrumentation(x => x.SetDbStatementForText = true);
    });

var app = builder.Build();

app.UseCors(x => x.AllowAnyOrigin());

app.MapGet("/podcasts", async (
    PodcastDbContext db, 
    RatingServiceHttpClient ratingServiceHttpClient,
    Tracer tracer) =>
{
    var all = await db.Podcasts
        .OrderBy(x => x.Title)
        .ToListAsync();

    var withRatings = new List<PodcastApiModel>();



    using (var span = tracer.StartActiveSpan("Get ratings from rating service"))
    {
        foreach (var podcast in all)
        {
            withRatings.Add(new PodcastApiModel(
                podcast.Title,
                await ratingServiceHttpClient.GetRating(podcast.Title)
            ));
        }

        //span.SetStatus(Status.Error);
        //span.RecordException();
        //trace going across messaging for pub/sub, not only https grpc
        //be careful with spans that would sit days long
        //thats where linkage comes in
        //most messaging your firing a message and something picking it up straight away
        //seeing that in a trace
        //see the front end, calling an api, which then fires a message, then a worker picks it up and that worker maybe calls a database
        //seeing all this in one trace is powerful

        //Easy to add information to span
        span.SetAttribute("Count", all.Count);
    }

    //using (tracer.StartActiveSpan("Get ratings from rating service"))
    //{
    //    foreach (var podcast in all)
    //    {
    //        withRatings.Add(new PodcastApiModel(
    //            podcast.Title,
    //            await ratingServiceHttpClient.GetRating(podcast.Title)
    //        ));
    //    }
    //}

    //using var span = tracer.StartActiveSpan("Get ratings from rating service");
    //foreach (var podcast in all)
    //{
    //    withRatings.Add(new PodcastApiModel(
    //        podcast.Title,
    //        await ratingServiceHttpClient.GetRating(podcast.Title)
    //    ));
    //}

    return withRatings;
});

var demoMeter = new Meter("Dometrain.AspireCourse", "1.0");
var ratingsSubmittedCounter = demoMeter.CreateCounter<int>("ratings_submitted");

app.MapPost("/rating", async (RatingServiceHttpClient ratingServiceHttpClient, [FromBody] PodcastApiModel podcast) =>
{
    await ratingServiceHttpClient.SubmitRating(podcast.PodcastName, podcast.Rating);

    ratingsSubmittedCounter.Add(1);
});

app.MapDefaultEndpoints();

app.Run();

record PodcastApiModel(string PodcastName, int Rating);
