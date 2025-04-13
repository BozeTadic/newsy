using Microsoft.EntityFrameworkCore;

namespace Newsy.Api.Infrastructure.Persistence;

public interface IDbContext : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
}