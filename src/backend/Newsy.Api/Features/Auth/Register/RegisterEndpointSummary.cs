using FastEndpoints;
using FluentValidation.Results;

namespace Newsy.Api.Features.Auth.Register;

public class RegisterEndpointSummary : Summary<RegisterEndpoint>
{
    public RegisterEndpointSummary()
    {
        Summary = "Sign up an author on our service";
        Response<ValidationFailure>(400, "Validation failed, check errors");
    }
}