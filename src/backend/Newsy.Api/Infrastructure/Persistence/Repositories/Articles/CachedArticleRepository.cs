using Microsoft.Extensions.Caching.Hybrid;
using Newsy.Api.Domain;

namespace Newsy.Api.Infrastructure.Persistence.Repositories.Articles;

public class CachedArticleRepository : IArticleRepository
{
    private readonly ArticleRepository _decoratedRepository;
    private readonly HybridCache _cache;

    public CachedArticleRepository(ArticleRepository decoratedRepository, HybridCache cache)
    {
        _decoratedRepository = decoratedRepository;
        _cache = cache;
    }

    public async Task<Article?> GetAsync(int id)
    {
        return await _cache.GetOrCreateAsync($"article-{id}", async _ =>
            await _decoratedRepository.GetAsync(id), tags: [$"article-{id}"]);
    }

    public async Task<List<Article>> GetAllAsync(int pageNumber, int pageSize)
    {
        return await _cache.GetOrCreateAsync($"articles-page-{pageNumber}-size-{pageSize}", async _ =>
            await _decoratedRepository.GetAllAsync(pageNumber, pageSize), tags: ["articles"]);
    }

    public async Task<Article?> CreateAsync(Article article)
    {
        var createdArticle = await _decoratedRepository.CreateAsync(article);

        await _cache.RemoveByTagAsync(["articles", $"article-{article.Id}"]);

        return createdArticle;
    }
}