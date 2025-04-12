using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Newsy.Api.Infrastructure.Persistence;

namespace Newsy.Api.Features.Article;

public class GetArticlesEndpoint : EndpointWithoutRequest<List<ArticleResponse>, ArticleResponseMapper>
{
    private readonly NewsyDbContext _dbContext;

    public GetArticlesEndpoint(NewsyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Get("/api/articles");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var articles = await _dbContext.Articles.Include(a => a.Author).ToListAsync(ct);

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