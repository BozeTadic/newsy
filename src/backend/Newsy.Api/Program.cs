using Microsoft.EntityFrameworkCore;
using Newsy.Api.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


// Add DbContext
builder.Services.AddDbContext<NewsyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("LocalPostgresConnectionString")));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();