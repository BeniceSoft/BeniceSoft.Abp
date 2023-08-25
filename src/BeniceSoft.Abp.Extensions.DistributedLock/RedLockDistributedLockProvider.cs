using BeniceSoft.Abp.Extensions.DistributedLock.Abstractions;
using Microsoft.Extensions.Options;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.Extensions.DistributedLock;

public class RedLockDistributedLockProvider : IDistributedLockProvider, ITransientDependency
{
    private readonly List<IRedLock> _managedLocks;
    private readonly IConnectionMultiplexer _multiplexer;
    private readonly RedLockFactory _redLockFactory;

    public RedLockDistributedLockProvider(IOptions<DistributedLockOptions> options)
    {
        var setting = options.Value;
        _multiplexer = ConnectionMultiplexer.Connect(setting.ConnectionString);
        _redLockFactory = RedLockFactory.Create(new List<RedLockMultiplexer> {new(_multiplexer)});
        _managedLocks = new();
    }

    public virtual async Task<bool> TryAcquireAsync(string resourceId, TimeSpan expires, bool throwOnFail)
    {
        if (_redLockFactory == null)
            throw new InvalidOperationException();

        var redLock = await _redLockFactory.CreateLockAsync(resourceId, expires);
        if (redLock.IsAcquired)
        {
            lock (_managedLocks)
            {
                _managedLocks.Add(redLock);
            }

            return true;
        }

        if (throwOnFail) throw new SynchronizationLockException($"获取分布式互斥锁失败: {resourceId}");
        return false;
    }


    public virtual async Task<bool> AcquireAsync(string resourceId, TimeSpan expires, TimeSpan wait, TimeSpan interval,
        bool throwOnFail)
    {
        var waitMillisecond = Convert.ToUInt32(wait.TotalMilliseconds);
        var startTime = Environment.TickCount;
        do
        {
            var result = await TryAcquireAsync(resourceId, expires, false);
            if (result) return result;

            SpinWait.SpinUntil(() => Environment.TickCount - startTime <= waitMillisecond, interval);
        } while (Environment.TickCount - startTime <= waitMillisecond);

        if (throwOnFail) throw new SynchronizationLockException($"获取分布式互斥锁失败: {resourceId}");
        return false;
    }

    public Task ReleaseLockAsync(string resourceId)
    {
        if (_redLockFactory == null)
            throw new InvalidOperationException();

        lock (_managedLocks)
        {
            foreach (var redLock in _managedLocks)
            {
                if (redLock.Resource == resourceId)
                {
                    redLock.Dispose();
                    _managedLocks.Remove(redLock);
                    break;
                }
            }
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _multiplexer.Dispose();
        _redLockFactory.Dispose();
    }
}