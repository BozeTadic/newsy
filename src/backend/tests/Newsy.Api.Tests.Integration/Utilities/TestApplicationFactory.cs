using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Newsy.Api.Infrastructure.Persistence;
using Newsy.Api.Infrastructure.Persistence.Repositories.Articles;
using Newsy.Api.Infrastructure.Persistence.Repositories.Authors;

namespace Newsy.Api.Tests.Integration.Utilities;

public class TestApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            var dbDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<NewsyDbContext>));

            if (dbDescriptor != null)
            {
                services.Remove(dbDescriptor);
            }

            var provider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            services.AddDbContext<IDbContext, NewsyDbContext>(options =>
            {
                options.UseInMemoryDatabase("newsy-in-memory-db");
                options.UseInternalServiceProvider(provider);
            });
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();

            var cacheDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(HybridCache));

            if (cacheDescriptor != null)
            {
                services.Remove(cacheDescriptor);
            }

            var fakeCache = new FakeHybridCache();
            services.AddSingleton<HybridCache>(fakeCache);
        });

        base.ConfigureWebHost(builder);
    }
}