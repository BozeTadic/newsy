using FastEndpoints;
using System.Security.Claims;
using Newsy.Api.Infrastructure.Persistence;

namespace Newsy.Api.Features.Article;

public class CreateArticleEndpoint : Endpoint<CreateArticleRequest>
{
    private readonly NewsyDbContext _dbContext;

    public CreateArticleEndpoint(NewsyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Post("/api/article");
    }

    public override async Task HandleAsync(CreateArticleRequest req, CancellationToken ct)
    {
        var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var article = new Domain.Article
        {
            Title = req.Title,
            Content = req.Content,
            AuthorId = int.Parse(userId)
        };

        var createdArticle = await _dbContext.Articles.AddAsync(article, ct);
        await _dbContext.SaveChangesAsync(ct);

        
        await SendCreatedAtAsync<GetArticleEndpoint>($"api/articles/{createdArticle.Entity.Id}",
            new ArticleResponse(createdArticle.Entity.Id, article.Title, article.Content, null), cancellation: ct);
    }
}