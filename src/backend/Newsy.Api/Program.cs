using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newsy.Api.Infrastructure.Persistence;
using FastEndpoints;
using FastEndpoints.Swagger;
using Newsy.Api.Infrastructure.Persistence.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


builder.Services
    .AddDbContext<IDbContext, NewsyDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("LocalPostgresConnectionString")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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
}

app.UseHttpsRedirection();
app.UseFastEndpoints();

app.Run();