var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Presentation>("presentation");

builder.Build().Run();