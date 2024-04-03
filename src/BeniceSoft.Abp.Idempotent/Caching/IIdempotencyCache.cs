namespace BeniceSoft.Abp.Idempotent.Caching;

public interface IIdempotencyCache
{
    Task<IdempotencyCacheEntry> GetOrDefaultAsync(string key, CancellationToken cancellationToken = default);

    Task SetAsync(string key, IdempotencyCacheEntry entry, CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    Task<IdempotencyCacheEntry> GetOrSetAsync(string key, IdempotencyCacheEntry entry, CancellationToken cancellationToken = default);
}