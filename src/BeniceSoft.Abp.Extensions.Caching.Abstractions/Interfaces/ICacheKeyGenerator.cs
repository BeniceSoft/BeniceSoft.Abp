using Volo.Abp.DynamicProxy;

namespace BeniceSoft.Abp.Extensions.Caching.Abstractions.Interfaces;

/// <summary>
/// 缓存键生成
/// </summary>
public interface ICacheKeyGenerator
{
    /// <summary>
    /// 生成缓存键
    /// </summary>
    /// <param name="invocation"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    string Generate(IAbpMethodInvocation invocation, string? key);
}