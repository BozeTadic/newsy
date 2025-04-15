using Newsy.Api.Infrastructure.Persistence.Repositories.Articles;
using Newsy.Api.Infrastructure.Persistence.Repositories.Authors;

namespace Newsy.Api.Infrastructure.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbContext _dbContext;
    public IArticleRepository ArticleRepository { get; }
    public IAuthorRepository AuthorRepository { get; }

    public UnitOfWork(IDbContext context, IArticleRepository articleRepository, IAuthorRepository authorRepository)
    {
        _dbContext = context;
        ArticleRepository = articleRepository;
        AuthorRepository = authorRepository;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}