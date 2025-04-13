using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("password", secret: true);

// What we've done so far, which uses a container both locally and when publishing...
var server = builder.AddSqlServer("server", password, 1433)
    .WithLifetime(ContainerLifetime.Persistent);

// The following demonstrate using the Azure SQL integration for different use-cases. Note that
// your Azure subscription configuration need to be set.
// See: https://learn.microsoft.com/en-us/dotnet/aspire/azure/local-provisioning#configuration

// Spin up an Azure SQL Server instance (both locally and when publishing).
// var server = builder.AddAzureSqlServer("server");

// Spin up Azure SQL Server when publishing, but use a container for local development...
// var server = builder.AddAzureSqlServer("server")
//     .RunAsContainer();

// Use existing Azure database in our subscription when running locally.
// When publishing, it'll create if it doesn't exist.
// var server = builder.AddAzureSqlServer("server")
//     .RunAsExisting("existing-resource-name", "existing-resource-group-name");

// Use existing Azure database in our subscription when publishing.
// When running locally, it'll create if it doesn't exist.
// var server = builder.AddAzureSqlServer("server")
//     .PublishAsExisting("existing-resource-name", "existing-resource-group-name");

// Use existing Azure database in our subscription for BOTH running and publishing.
// var existingResourceName = builder.AddParameter("sql-resource-name");
// var server = builder.AddAzureSqlServer("server")
//     .AsExisting(existingResourceName, null);

// Use container locally.
// When publishing, we're just specifying a connection string in config, rather than
// creating a new resource. This doesn't have to be in our Azure Subscription, unlike the above.
// var server = builder.AddAzureSqlServer("server")
//     .RunAsContainer()
//     .PublishAsConnectionString();

if (builder.ExecutionContext.IsRunMode)
    Console.WriteLine("We're running our app locally!");

if (builder.ExecutionContext.IsPublishMode)
    Console.WriteLine("Our app is being published!");

var db = server
    .AddDatabase("podcasts");

var cache = builder.AddRedis("cache")
    .WithRedisCommander()
    .WithLifetime(ContainerLifetime.Persistent);

var ratingService = builder.AddProject<RatingService>("ratingservice")
    .WithReference(cache)
    .WaitFor(cache);

var api = builder.AddProject<Api>("api")
    .WithReference(db)
    .WithReference(ratingService)
    .WaitFor(db)
    .WaitFor(ratingService);

builder.AddProject<Frontend>("frontend")
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

builder.AddProject<MigrationService>("migration")
    .WithReference(db)
    .WaitFor(db)
    .WithParentRelationship(server);

builder.AddDockerComposePublisher();

builder.Build().Run();