using FastEndpoints;
using Newsy.Api.Infrastructure.Persistence.UnitOfWork;

namespace Newsy.Api.Features.Articles;

public class GetArticlesEndpoint : EndpointWithoutRequest<List<ArticleResponse>, ArticleResponseMapper>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetArticlesEndpoint(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override void Configure()
    {
        Get("/api/articles");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var articles = await _unitOfWork.ArticleRepository.GetAllAsync();

        if (articles.Count == 0)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var response = articles
            .Select(Map.FromEntity)
            .ToList();

        await SendOkAsync(response, ct);
    }
}