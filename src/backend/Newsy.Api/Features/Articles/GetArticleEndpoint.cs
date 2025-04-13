using FastEndpoints;
using Microsoft.Extensions.Caching.Hybrid;
using Newsy.Api.Infrastructure.Persistence.UnitOfWork;

namespace Newsy.Api.Features.Articles;

public class GetArticleEndpoint : EndpointWithoutRequest<ArticleResponse, ArticleResponseMapper>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly HybridCache _cache;

    public GetArticleEndpoint(IUnitOfWork unitOfWork, HybridCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
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

        var cacheKey = $"article-{id}";

        var cachedResponse = await _cache.GetOrCreateAsync(cacheKey, async _ =>
        {
            var article = await _unitOfWork.ArticleRepository.GetAsync(id);

            return article == null ? null : Map.FromEntity(article);
        },
            cancellationToken: ct);

        if (cachedResponse == null)
        {
            await SendNotFoundAsync(ct);
            await _cache.RemoveAsync(cacheKey, ct);
            return;
        }

        await SendOkAsync(cachedResponse, ct);
    }
}