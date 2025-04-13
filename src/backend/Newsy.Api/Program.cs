using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newsy.Api.Infrastructure.Persistence;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.Extensions.Caching.Hybrid;
using Newsy.Api.Infrastructure.Persistence.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


builder.Services
    .AddDbContext<IDbContext, NewsyDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("LocalPostgresConnectionString")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddHybridCache(options => 
    options.DefaultEntryOptions = new HybridCacheEntryOptions()
    {
        LocalCacheExpiration = TimeSpan.FromMinutes(5),
        Expiration = TimeSpan.FromMinutes(15)
    }
);

builder.AddRedisDistributedCache("newsy-redis");

builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddFastEndpoints().SwaggerDocument(options =>
{
    options.EnableJWTBearerAuth = true;
    options.ShortSchemaNames = true;
    options.DocumentSettings = d => d.MarkNonNullablePropsAsRequired();
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<NewsyDbContext>();
    dbContext.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseDefaultExceptionHandler();
app.UseFastEndpoints();

app.Run();