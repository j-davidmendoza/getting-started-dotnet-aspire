using Projects;

var builder = DistributedApplication.CreateBuilder(args);

///////////////////////////
// Some extra integrations, just for fun...

var redis = builder.AddRedis("cache")
    .WithRedisCommander();

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();

var mongoDb = builder.AddMongoDB("mongodb")
    .WithMongoExpress();

///////////////////////////

var password = builder.AddParameter("password", secret: true);

var server = builder.AddSqlServer("server", password, 1433)
    .WithLifetime(ContainerLifetime.Persistent);

var db = server
    .AddDatabase("podcasts");

var api = builder.AddProject<Api>("api")
    .WithReference(db)
    .WithHttpEndpoint(name: "dashboard")
    .WaitFor(db);

builder.AddProject<Frontend>("frontend")
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

builder.AddProject<MigrationService>("migration")
    .WithReference(db)
    .WaitFor(db)
    .WithParentRelationship(server);

builder.Build().Run();