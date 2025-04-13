using Newsy.Api.Infrastructure.Persistence.Repositories.Articles;
using Newsy.Api.Infrastructure.Persistence.Repositories.Authors;

namespace Newsy.Api.Infrastructure.Persistence.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IArticleRepository ArticleRepository { get; }
    IAuthorRepository AuthorRepository { get; }
    Task SaveChangesAsync();
}