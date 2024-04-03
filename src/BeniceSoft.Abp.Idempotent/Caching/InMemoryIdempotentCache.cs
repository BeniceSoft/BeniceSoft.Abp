using System.Collections.Concurrent;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.Idempotent.Caching;

[ExposeServices(typeof(IIdempotencyCache))]
public class InMemoryIdempotentCache : IIdempotencyCache, ISingletonDependency
{
    private readonly ConcurrentDictionary<string, IdempotencyCacheEntry> _dictionary = new();

    public Task<IdempotencyCacheEntry> GetOrDefaultAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_dictionary.GetOrDefault(key));
    }

    public Task SetAsync(string key, IdempotencyCacheEntry entry, CancellationToken cancellationToken = default)
    {
        _dictionary.AddOrUpdate(key, _ => entry, (_, _) => entry);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _dictionary.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public Task<IdempotencyCacheEntry> GetOrSetAsync(string key, IdempotencyCacheEntry entry, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_dictionary.GetOrAdd(key, entry));
        
    }
}