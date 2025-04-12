var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Newsy_Api>("newsy-api");

builder.Build().Run();
