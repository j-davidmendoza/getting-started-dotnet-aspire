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
    .WithHttpsEndpoint(port: 1234) //This is an explict binding, if not aspire will do implict bindings here it expects appurl in launchsettings, can also use kestral
    // If not appurl nor explict, then no binding behind api
    //.WithHttpEndpoint(name: "dashboard")
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