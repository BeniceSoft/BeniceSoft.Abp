namespace BeniceSoft.Abp.Extensions.DistributedLock.Abstractions;

/// <summary>
/// 分布式锁
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class DistributedLockAttribute : Attribute
{
    /// <summary>
    /// 资源id
    /// </summary>
    public string ResourceId { get; set; } = string.Empty;
    
    /// <summary>
    /// 过期毫秒数 默认1min
    /// </summary>
    public int ExpiresMilliseconds { get; set; } = 60000;
    
    /// <summary>
    /// 等待毫秒数 默认100ms
    /// </summary>
    public int WaitMilliseconds { get; set; } = 100;
    
    /// <summary>
    /// 循环间隔时间 默认25ms
    /// </summary>
    public int IntervalMilliseconds { get; set; } = 25;
}