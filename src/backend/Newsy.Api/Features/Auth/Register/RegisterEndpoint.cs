using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Newsy.Api.Infrastructure.Persistence;

namespace Newsy.Api.Features.Auth.Register;

public class RegisterEndpoint : EndpointWithMapper<RegisterRequest, RegisterAuthorMapper>
{
    private readonly NewsyDbContext _dbContext;

    public RegisterEndpoint(NewsyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Post("/api/auth/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        if (await _dbContext.Authors.AnyAsync(a => a.Username == req.Username, cancellationToken: ct))
        {
            AddError("User already exists.");
            await SendErrorsAsync(400, ct);
            return;
        }

        var author = Map.ToEntity(req);

        await _dbContext.Authors.AddAsync(author, ct);
        await _dbContext.SaveChangesAsync(ct);

        await SendOkAsync(ct);
    }
}