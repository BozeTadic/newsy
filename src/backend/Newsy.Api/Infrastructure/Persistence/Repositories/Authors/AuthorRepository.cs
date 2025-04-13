using Microsoft.EntityFrameworkCore;
using Newsy.Api.Domain;

namespace Newsy.Api.Infrastructure.Persistence.Repositories.Authors;

internal class AuthorRepository : IAuthorRepository
{
    private readonly IDbContext _dbContext;
    public AuthorRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Author?> GetAsync(string username)
    {
        return await _dbContext.Set<Author>().FirstOrDefaultAsync(a => a.Username == username);
    }

    public async Task<bool> CreateAsync(Author author)
    {
        await _dbContext.Set<Author>().AddAsync(author);

        return await _dbContext.SaveChangesAsync() > 0;
    }
}