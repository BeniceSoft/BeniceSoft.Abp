using BeniceSoft.Abp.OperationLogging.Abstractions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;

namespace BeniceSoft.Abp.OperationLogging;

public class OperationLogInterceptorRegister
{
    public static void RegisterIfNeeded(IOnServiceRegistredContext context)
    {
        if (ShouldIntercept(context.ImplementationType))
        {
            context.Interceptors.TryAdd<OperationLogInterceptor>();
        }
    }

    private static bool ShouldIntercept(Type type)
    {
        // 是否是动态代理忽略的类型（默认中包含 controller 需注意）
        if (DynamicProxyIgnoreTypes.Contains(type))
        {
            return false;
        }

        // 方法有 OperationLogAttribute 标签
        if (type.GetMethods().Any(m => m.IsDefined(typeof(OperationLogAttribute), true)))
        {
            return true;
        }

        return false;
    }
}