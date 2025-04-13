using FastEndpoints;

namespace Newsy.Api.Features.Articles;

public class ArticleRequestMapper : RequestMapper<CreateArticleRequest, Domain.Article>
{
    public Domain.Article ToEntity(CreateArticleRequest r, string userId) => new()
    {
        Title = r.Title,
        Content = r.Content,
        AuthorId = int.Parse(userId)
    };
}

public class ArticleResponseMapper : ResponseMapper<ArticleResponse, Domain.Article>
{
    public override ArticleResponse FromEntity(Domain.Article e) => new(e.Id, e.Title, e.Content, e.Author?.Username);
}