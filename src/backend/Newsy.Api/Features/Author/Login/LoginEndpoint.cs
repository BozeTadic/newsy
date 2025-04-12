using FastEndpoints;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newsy.Api.Infrastructure.Persistence;

namespace Newsy.Api.Features.Author.Login;

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly NewsyDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public LoginEndpoint(NewsyDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var author = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Username == req.Username, cancellationToken: ct);

        if (author == null || !BCrypt.Net.BCrypt.EnhancedVerify(req.Password, author.PasswordHash))
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, author.Id.ToString()),
            new Claim(ClaimTypes.Name, author.Username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JwtSettings:SecretKey")!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration.GetValue<string>("JwtSettings:Issuer"),
            audience: _configuration.GetValue<string>("JwtSettings:Audience"),
            expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JwtSettings:TokenExpirationTimeInMinutes")),
            claims: claims,
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        await SendAsync(new LoginResponse(tokenString), cancellation: ct);
    }
}