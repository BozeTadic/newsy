using Newsy.Api.Domain;

namespace Newsy.Api.Infrastructure.Persistence.Repositories.Authors;

public interface IAuthorRepository
{
    Task<Author?> GetAsync(string username);

    Task<bool> CreateAsync(Author author);
}