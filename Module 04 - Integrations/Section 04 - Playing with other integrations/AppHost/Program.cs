using Projects;

var builder = DistributedApplication.CreateBuilder(args);

//Parameters allow the app to ask for external values when running the app
//values can come from any usual configuration source
//or when publishing
//if value not available, the app will prompt you

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

//Aspire knows to call, .WithReference, it knows to call interface to get connection string to inject ennvironment variables to tell service where target endpoint is
var myParameter = builder.AddParameter("myParameter");

var myConnectionString = builder.AddConnectionString("myConnectionString");

var server = builder.AddSqlServer("server", password, 1433)
    .WithLifetime(ContainerLifetime.Persistent);

var db = server
    .AddDatabase("podcasts");

var api = builder.AddProject<Api>("api")
    .WithReference(db)
    //.WithHttpsEndpoint(port: 1234) //This is an explict binding, if not aspire will do implict bindings here it expects appurl in launchsettings, can also use kestral
    // If not appurl nor explict, then no binding behind api
    //.WithHttpEndpoint(name: "dashboard")
    .WaitFor(db)
    .WithReference(myConnectionString)
    .WithEnvironment("MY_ENVIRONMENT_VARIABLE", myParameter);//this setting environment variables 

builder.AddProject<Frontend>("frontend")
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

builder.AddProject<MigrationService>("migration")
    .WithReference(db)
    .WaitFor(db)
    .WithParentRelationship(server);

builder.Build().Run();