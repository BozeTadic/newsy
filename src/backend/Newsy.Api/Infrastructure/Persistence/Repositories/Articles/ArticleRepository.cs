using Microsoft.EntityFrameworkCore;
using Newsy.Api.Domain;

namespace Newsy.Api.Infrastructure.Persistence.Repositories.Articles;

public class ArticleRepository : IArticleRepository
{
    private readonly IDbContext _dbContext;
    public ArticleRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Article?> GetAsync(int id)
    {
        return await _dbContext.Set<Article>()
            .Include(a => a.Author)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<Article>> GetAllAsync(int pageNumber, int pageSize)
    {
        return await _dbContext.Set<Article>()
            .Include(a => a.Author)
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Article?> CreateAsync(Article article)
    {
        await _dbContext.Set<Article>().AddAsync(article);

        return await _dbContext.SaveChangesAsync() > 0 ? article : null;
    }
}