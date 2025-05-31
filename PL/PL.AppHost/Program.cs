var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.PL_ApiService>("apiservice");

builder.AddProject<Projects.PL_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
