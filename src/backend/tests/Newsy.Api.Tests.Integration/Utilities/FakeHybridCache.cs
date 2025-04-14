using Microsoft.Extensions.Caching.Hybrid;

namespace Newsy.Api.Tests.Integration.Utilities;

public class FakeHybridCache : HybridCache
{
    public override ValueTask RemoveByTagAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default)
    {
        return new ValueTask();
    }

    public override ValueTask RemoveByTagAsync(string tag, CancellationToken cancellationToken = new ())
    {
        return new ValueTask();
    }

    public override async ValueTask<T> GetOrCreateAsync<TState, T>(string key, TState state,
        Func<TState, CancellationToken, ValueTask<T>> factory, HybridCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null, CancellationToken cancellationToken = default)
    {
        return await factory(state, cancellationToken);
    }

    public override ValueTask SetAsync<T>(string key, T value, HybridCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null, CancellationToken cancellationToken = default)
    {
        return new ValueTask();
    }

    public override ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        return new ValueTask();
    }
}