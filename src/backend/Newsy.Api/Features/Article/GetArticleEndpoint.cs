using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Newsy.Api.Infrastructure.Persistence;

namespace Newsy.Api.Features.Article;

public class GetArticleEndpoint : EndpointWithoutRequest<ArticleResponse, ArticleResponseMapper>
{
    private readonly NewsyDbContext _dbContext;

    public GetArticleEndpoint(NewsyDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Get("/api/articles/{id}");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<int>("id");

        var article = await _dbContext.Articles
            .Include(a => a.Author)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

        if (article == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var response = Map.FromEntity(article);

        await SendOkAsync(response, ct);
    }
}