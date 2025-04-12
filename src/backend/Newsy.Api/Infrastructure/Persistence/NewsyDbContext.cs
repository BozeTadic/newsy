using Microsoft.EntityFrameworkCore;
using Newsy.Api.Domain;

namespace Newsy.Api.Infrastructure.Persistence;

public class NewsyDbContext(DbContextOptions<NewsyDbContext> options) : DbContext(options)
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Article> Articles { get; set; }
}