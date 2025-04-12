using FastEndpoints;
using FluentValidation;

namespace Newsy.Api.Features.Auth.Register;

public class RegisterRequestValidator : Validator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("username is required")
            .MinimumLength(3)
            .WithMessage("username too short");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("password is required")
            .MinimumLength(8)
            .WithMessage("password must be at least 8 characters long")
            .Matches("[A-Z]")
            .WithMessage("password must contain at least one uppercase letter")
            .Matches("[a-z]")
            .WithMessage("password must contain at least one lowercase letter")
            .Matches("[0-9]")
            .WithMessage("password must contain at least one number")
            .Matches("[^a-zA-Z0-9]")
            .WithMessage("password must contain at least one special character");
    }
}