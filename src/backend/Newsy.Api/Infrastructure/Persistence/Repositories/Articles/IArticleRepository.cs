using Newsy.Api.Domain;

namespace Newsy.Api.Infrastructure.Persistence.Repositories.Articles;

public interface IArticleRepository
{
    Task<Article?> GetAsync(int id);

    Task<List<Article>> GetAllAsync();

    Task<Article?> CreateAsync(Article article);
}