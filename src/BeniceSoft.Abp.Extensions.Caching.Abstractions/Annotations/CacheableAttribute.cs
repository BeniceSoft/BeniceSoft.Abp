namespace BeniceSoft.Abp.Extensions.Caching.Abstractions.Annotations;

/// <summary>
/// 方法返回值可缓存
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class CacheableAttribute : Attribute
{
    /// <summary>
    /// 缓存键，接受一个结果为 string 的表达式
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// 自定义缓存键生成类型，须实现 ICacheKeyGenerator 接口
    /// </summary>
    public Type? KeyGeneratorType { get; set; }

    /// <summary>
    /// 接收一个结果为 bool 的表达式作为执行方法缓存之前的判断，例如：
    /// <code>
    /// name == "abc" || id == 1
    /// </code>
    /// </summary>
    public string? Condition { get; set; }

    /// <summary>
    /// 接收一个结果为 bool 的表达式作为方法执行后是否缓存的判断，例如：
    /// <code>
    /// name != "abc"
    /// </code>
    /// </summary>
    public string? Unless { get; set; }

    /// <summary>
    /// 过期时长(秒)，注意：最终的缓存有效时间会在此时长基础上增加随机（30-60）秒数
    /// </summary>
    public int ExpirationSeconds { get; set; }
}