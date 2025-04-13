using Microsoft.EntityFrameworkCore;
using Newsy.Api.Domain;

namespace Newsy.Api.Infrastructure.Persistence;

internal class NewsyDbContext(DbContextOptions<NewsyDbContext> options) : DbContext(options), IDbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Article> Articles { get; set; }
}