var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.Api>("api")
    .WithReplicas(3) // this will create 3 replicas of the api project
    .WithExplicitStart(); // this will not start the api project automatically, you will have to start it manually later

builder.AddProject<Projects.Frontend>("frontend")
    .WithReference(api)// this extension methods lets you referenece name api instead of using localhost with port number
    .WaitFor(api) // wait for api to be up and running
    .WithExternalHttpEndpoints(); //useful for deployment later, basically means that the frontend is the publically accessible endpoint


builder.Build().Run();
