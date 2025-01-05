var builder = DistributedApplication.CreateBuilder(args);

var applicationProject = builder.AddProject<Projects.Application>("application");
var infrastructureProject = builder
    .AddProject<Projects.Infrastructure>("infrastructure")
    .WithReference(applicationProject);
builder.AddProject<Projects.Presentation>("presentation").WithReference(infrastructureProject);

builder.Build().Run();
