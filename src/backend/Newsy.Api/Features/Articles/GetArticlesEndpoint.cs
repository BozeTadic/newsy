using FastEndpoints;
using Microsoft.Extensions.Caching.Hybrid;
using Newsy.Api.Common.Pagination;
using Newsy.Api.Infrastructure.Persistence.UnitOfWork;

namespace Newsy.Api.Features.Articles;

public class GetArticlesEndpoint : Endpoint<PaginationQuery, PaginationResult<ArticleResponse>, ArticleResponseMapper>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly HybridCache _cache;

    public GetArticlesEndpoint(IUnitOfWork unitOfWork, HybridCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public override void Configure()
    {
        Get("/api/articles");
        AllowAnonymous();
    }

    public override async Task HandleAsync(PaginationQuery query, CancellationToken ct)
    {   
        var cacheKey = $"articles-page-{query.PageNumber}-size-{query.PageSize}";

        var response = await _cache.GetOrCreateAsync(cacheKey, async _ =>
        {
            var articles = await _unitOfWork.ArticleRepository.GetAllAsync(query.PageNumber, query.PageSize);

            if (articles.Count == 0)
            {
                return null;
            }

            return new PaginationResult<ArticleResponse>(articles.Select(Map.FromEntity).ToList(),
                query.PageNumber,
                query.PageSize
            );
        }, 
            tags: ["articles"],
            cancellationToken: ct);

        if (response == null)
        {
            await SendNotFoundAsync(ct);
            await _cache.RemoveAsync(cacheKey, ct);
            return;
        }

        await SendOkAsync(response, ct);
    }
}