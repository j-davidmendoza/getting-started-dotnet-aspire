using Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<PodcastDbContext>(connectionName: "podcasts");

var app = builder.Build();

app.UseCors(x => x.AllowAnyOrigin());

app.MapGet("/podcasts", async (PodcastDbContext db) => await db.Podcasts
    .OrderBy(x => x.Title)
    .Select(x => x.Title)
    .ToListAsync());

app.MapDefaultEndpoints();

app.Run();
