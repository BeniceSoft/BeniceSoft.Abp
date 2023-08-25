using BeniceSoft.Abp.Extensions.Caching.Configurations;
using BeniceSoft.Abp.Extensions.Caching.Abstractions.Annotations;

namespace BeniceSoft.Abp.Extensions.Caching.Extensions;

public static class CacheableAttributeExtensions
{
    public static TimeSpan GetExpireOn(this CacheableAttribute cacheableAttribute)
    {
        // 随机秒数
        var random = new Random();
        var seconds = random.Next(30, 60);

        // 未单独设置过期时间
        if (cacheableAttribute.ExpirationSeconds <= 0)
        {
            // 取默认过期时间(最少1秒)
            seconds += Math.Max(1, BeniceSoftCachingConfiguration.Instance.DefaultExpirationSeconds);
        }
        else
        {
            seconds += cacheableAttribute.ExpirationSeconds;
        }

        return TimeSpan.FromSeconds(seconds);
    }
}