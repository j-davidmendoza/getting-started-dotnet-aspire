using Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCors();

builder.AddSqlServerDbContext<PodcastDbContext>(connectionName: "podcasts");
//We canget rid of the connection string from the appsettings.json file and the below code witht he connectionname extension metho
//Just call rational name of the database name
//Done behind the scenes by the extension method with just one line of code
//var connectionString =
//    builder.Configuration.GetConnectionString("DefaultConnection")
//    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
////We are using native EF  Db context 
//builder.Services.AddDbContext<PodcastDbContext>(options =>
//    options.UseSqlServer(connectionString));

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseCors(x => x.AllowAnyOrigin());

app.MapGet("/podcasts", async (PodcastDbContext db) => await db.Podcasts
    .OrderBy(x => x.Title)
    .Select(x => x.Title)
    .ToListAsync());

app.Run();
