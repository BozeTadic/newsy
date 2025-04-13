using Microsoft.EntityFrameworkCore;
using Newsy.Api.Domain;

namespace Newsy.Api.Infrastructure.Persistence;

internal class NewsyDbContext(DbContextOptions<NewsyDbContext> options) : DbContext(options), IDbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Article> Articles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.Property(a => a.Username)
                .HasMaxLength(64)
                .IsRequired();

            entity.Property(a => a.PasswordHash)
                .HasMaxLength(64)
                .IsRequired();
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.Property(a => a.Title)
                .HasMaxLength(128)
                .IsRequired();

            entity.Property(a => a.Content)
                .HasMaxLength(4096)
                .IsRequired();
        });
    }
}