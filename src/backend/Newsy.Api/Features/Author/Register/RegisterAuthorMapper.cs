using FastEndpoints;

namespace Newsy.Api.Features.Author.Register;

public class RegisterAuthorMapper: RequestMapper<RegisterRequest, Domain.Author>
{
    public override Domain.Author ToEntity(RegisterRequest r) => new()
    {
        Username = r.Username,
        PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(r.Password)
    };
}