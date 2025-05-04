using Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCors();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<PodcastDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseCors(x => x.AllowAnyOrigin());

app.MapGet("/podcasts", async (PodcastDbContext db) => await db.Podcasts
    .OrderBy(x => x.Title)
    .Select(x => x.Title)
    .ToListAsync());

app.Run();
