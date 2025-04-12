namespace Newsy.Api.Features.Article;

public record ArticleResponse(int Id, string Title, string Content, string? AuthorName);