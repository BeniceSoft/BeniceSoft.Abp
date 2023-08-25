namespace BeniceSoft.Abp.Extensions.DistributedLock.Abstractions;

public interface IDistributedLockProvider : IDisposable
{
    /// <summary>
    /// 分配锁
    /// </summary>
    /// <param name="resourceId">资源id</param>
    /// <param name="expires">过期时间</param>
    /// <param name="wait">等待时间</param>
    /// <param name="interval">间隔时间</param>
    /// <param name="throwOnFail">失败抛出异常</param>
    /// <returns></returns>
    Task<bool> AcquireAsync(string resourceId, TimeSpan expires, TimeSpan wait, TimeSpan interval,
        bool throwOnFail);

    /// <summary>
    /// 尝试分配锁
    /// </summary>
    /// <param name="resourceId">资源id</param>
    /// <param name="expires">过期时间</param>
    /// <param name="throwOnFail">失败抛出异常</param>
    /// <returns></returns>
    Task<bool> TryAcquireAsync(string resourceId, TimeSpan expires, bool throwOnFail);

    /// <summary>
    /// 释放锁
    /// </summary>
    /// <param name="resourceId"></param>
    /// <returns></returns>
    Task ReleaseLockAsync(string resourceId);
}