using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Api>("api");

builder.AddProject<Frontend>("frontend")
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

builder.Build().Run();