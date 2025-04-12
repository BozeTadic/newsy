using FastEndpoints;
using Newsy.Api.Domain;

namespace Newsy.Api.Features.Auth.Register;

public class RegisterAuthorMapper: RequestMapper<RegisterRequest, Author>
{
    public override Author ToEntity(RegisterRequest r) => new()
    {
        Username = r.Username,
        PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(r.Password)
    };
}