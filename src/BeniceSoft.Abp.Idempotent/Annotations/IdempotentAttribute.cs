namespace BeniceSoft.Abp.Idempotent.Annotations;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class IdempotentAttribute : Attribute
{
    /// <summary>
    /// 是否可重复使用
    /// </summary>
    public bool IsReusable { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 过期时间（秒）
    /// </summary>
    public int ExpirationSeconds { get; set; }

    /// <summary>
    /// 只有在成功响应后才缓存
    /// </summary>
    public bool CacheOnlySuccessResponses { get; set; } = true;
}