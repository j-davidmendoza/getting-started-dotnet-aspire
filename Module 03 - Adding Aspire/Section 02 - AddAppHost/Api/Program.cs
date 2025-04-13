using Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.AddServiceDefaults();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<PodcastDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

app.UseCors(x => x.AllowAnyOrigin());

app.MapGet("/podcasts", async (PodcastDbContext db) => await db.Podcasts
    .OrderBy(x => x.Title)
    .Select(x => x.Title)
    .ToListAsync());

app.MapDefaultEndpoints();

app.Run();
