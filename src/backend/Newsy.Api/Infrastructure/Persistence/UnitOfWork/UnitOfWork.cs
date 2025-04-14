using Newsy.Api.Infrastructure.Persistence.Repositories.Articles;
using Newsy.Api.Infrastructure.Persistence.Repositories.Authors;

namespace Newsy.Api.Infrastructure.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbContext? _dbContext;
    private IArticleRepository? _articleRepository;
    private IAuthorRepository? _authorRepository;


    public UnitOfWork(IDbContext context)
    {
        _dbContext = context;
    }

    public IArticleRepository ArticleRepository
    {
        get { return _articleRepository ??= new ArticleRepository(_dbContext); }
    }

    public IAuthorRepository AuthorRepository
    {
        get { return _authorRepository ??= new AuthorRepository(_dbContext); }
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
    }
}