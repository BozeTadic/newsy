namespace Newsy.Api.Features.Articles;

public record ArticleResponse(int Id, string Title, string Content, string? AuthorName);