namespace BeniceSoft.Abp.Extensions.DistributedLock.Abstractions;

public class DistributedLockOptions
{
    /// <summary>
    /// 链接字符串
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}