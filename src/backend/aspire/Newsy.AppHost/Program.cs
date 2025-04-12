var builder = DistributedApplication.CreateBuilder(args);

// * Postgres *
var user = builder.AddParameter("postgres-user");
var password = builder.AddParameter("postgres-password", secret: true);

var postgresDbServer = builder.AddPostgres("newsy-postgres-server", user, password)
    .WithDataVolume(isReadOnly: false);

var newsyDb = postgresDbServer.AddDatabase("newsy-local-postgres");

builder.AddProject<Projects.Newsy_Api>("newsy-api")
    .WithReference(newsyDb)
    .WaitFor(newsyDb);

builder.Build().Run();
