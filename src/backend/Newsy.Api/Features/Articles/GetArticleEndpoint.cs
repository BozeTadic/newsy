using FastEndpoints;
using Newsy.Api.Infrastructure.Persistence.UnitOfWork;

namespace Newsy.Api.Features.Articles;

public class GetArticleEndpoint : EndpointWithoutRequest<ArticleResponse, ArticleResponseMapper>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetArticleEndpoint(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override void Configure()
    {
        Get("/api/articles/{id}");
        Options(x => x.WithName("GetArticleEndpoint"));
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<int>("id");

        var article = await _unitOfWork.ArticleRepository.GetAsync(id);

        if (article == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var response = Map.FromEntity(article);

        await SendOkAsync(response, ct);
    }
}