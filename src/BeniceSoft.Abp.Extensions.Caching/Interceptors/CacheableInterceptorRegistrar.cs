using BeniceSoft.Abp.Extensions.Caching.Abstractions.Annotations;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;

namespace BeniceSoft.Abp.Extensions.Caching.Interceptors;

public class CacheableInterceptorRegistrar
{
    public static void RegisterIfNeeded(IOnServiceRegistredContext context)
    {
        if (ShouldIntercept(context.ImplementationType))
        {
            context.Interceptors.TryAdd<CacheableInterceptor>();
        }
    }

    private static bool ShouldIntercept(Type type)
    {
        // 是否是动态代理忽略的类型
        if (DynamicProxyIgnoreTypes.Contains(type))
        {
            return false;
        }

        // 类上有 CacheableAttribute 标签
        if (type.IsDefined(typeof(CacheableAttribute), true))
        {
            return true;
        }

        // 方法有 CacheableAttribute 标签
        if (type.GetMethods().Any(m => m.IsDefined(typeof(CacheableAttribute), true)))
        {
            return true;
        }

        return false;
    }
}