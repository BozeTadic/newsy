using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newsy.Api.Domain;
using Newsy.Api.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


// Add DbContext
builder.Services.AddDbContext<NewsyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("LocalPostgresConnectionString")));

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/auth/register", async (NewsyDbContext db, UserRequestDto registerRequest) =>
{
    if (await db.Authors.AnyAsync(a => a.Username == registerRequest.Username))
    {
        return Results.BadRequest("Username already exists.");
    }

    var author = new Author
    {
        Username = registerRequest.Username,
        PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(registerRequest.Password)
    };

    await db.Authors.AddAsync(author);
    await db.SaveChangesAsync();

    return Results.Ok("Registered successfully.");
});

app.MapPost("/api/auth/login", async (NewsyDbContext db, IConfiguration config, UserRequestDto loginRequest) =>
{
    var author = await db.Authors.FirstOrDefaultAsync(a => a.Username == loginRequest.Username);

    if (author == null || !BCrypt.Net.BCrypt.EnhancedVerify(loginRequest.Password, author.PasswordHash))
    {
        return Results.Unauthorized();
    }

    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, author.Id.ToString()),
        new Claim(ClaimTypes.Name, author.Username)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("JwtSettings:SecretKey")!));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: config.GetValue<string>("JwtSettings:Issuer"),
        audience: config.GetValue<string>("JwtSettings:Audience"),
        expires: DateTime.UtcNow.AddMinutes(config.GetValue<int>("JwtSettings:TokenExpirationTimeInMinutes")),
        claims: claims,
        signingCredentials: credentials
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(new { token = tokenString });
});

app.MapGet("/api/articles", async (NewsyDbContext db) => await db.Articles.ToListAsync())
    .RequireAuthorization();


app.Run();

public record UserRequestDto(string Username, string Password);