using FastEndpoints;
using Newsy.Api.Infrastructure.Persistence.UnitOfWork;

namespace Newsy.Api.Features.Auth.Register;

public class RegisterEndpoint : EndpointWithMapper<RegisterRequest, RegisterAuthorMapper>
{
    private readonly IUnitOfWork _unitOfWork;

    public RegisterEndpoint(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override void Configure()
    {
        Post("/api/auth/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var registeredAuthor = await _unitOfWork.AuthorRepository.GetAsync(req.Username);
        if (registeredAuthor != null)
        {
            AddError("User already exists.");
            await SendErrorsAsync(400, ct);
            return;
        }

        var author = Map.ToEntity(req);

        bool authorCreated = await _unitOfWork.AuthorRepository.CreateAsync(author);

        if (!authorCreated)
        {
            AddError("Internal server error.");
            AddError("Failed to create user.");
            await SendErrorsAsync(500, ct);
            return;
        }

        await SendAsync(new EmptyResponse(), 201, ct);
    }
}