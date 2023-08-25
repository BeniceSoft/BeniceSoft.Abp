using System.Reflection;
using BeniceSoft.Abp.Extensions.DistributedLock.Abstractions;
using Microsoft.Extensions.Logging;
using SmartFormat;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;

namespace BeniceSoft.Abp.Extensions.DistributedLock;

public class DistributedLockInterceptor : AbpInterceptor, ITransientDependency
{
    private readonly ILogger<DistributedLockInterceptor> _logger;
    private readonly IDistributedLockProvider _distributedLockProvider;

    public DistributedLockInterceptor(ILogger<DistributedLockInterceptor> logger,
        IDistributedLockProvider distributedLockProvider)
    {
        _logger = logger;
        _distributedLockProvider = distributedLockProvider;
    }

    public override async Task InterceptAsync(IAbpMethodInvocation invocation)
    {
        var distributedLockAttribute =
            invocation.Method.GetCustomAttribute(typeof(DistributedLockAttribute)) as DistributedLockAttribute;
        if (distributedLockAttribute is null)
        {
            await invocation.ProceedAsync();
            return;
        }

        var resourceId = GetResourceId(distributedLockAttribute, invocation);

        // 分配锁
        await _distributedLockProvider.AcquireAsync(
            resourceId,
            TimeSpan.FromMilliseconds(distributedLockAttribute.ExpiresMilliseconds),
            TimeSpan.FromMilliseconds(distributedLockAttribute.WaitMilliseconds),
            TimeSpan.FromMilliseconds(distributedLockAttribute.IntervalMilliseconds),
            true);

        try
        {
            // 执行方法
            await invocation.ProceedAsync();
        }
        finally
        {
            // 释放锁
            await _distributedLockProvider.ReleaseLockAsync(resourceId);
        }
    }


    private string GetResourceId(DistributedLockAttribute attribute, IAbpMethodInvocation invocation)
    {
        // 未设置资源id 默认使用方法名作为资源id
        if (string.IsNullOrWhiteSpace(attribute.ResourceId))
        {
            var methodInfo = invocation.Method;
            return $"{methodInfo.DeclaringType?.Namespace}.{methodInfo.DeclaringType?.Name}.{methodInfo.Name}";
        }

        //var parameterExpressions = invocation.ArgumentsDictionary
        //    .Select(arg => Expression.Parameter(arg.Value.GetType(), arg.Key))
        //    .ToArray();

        //var expr = $"\"{attribute.ResourceId}\"";
        //var sourceExpr = DynamicExpressionParser.ParseLambda(parameterExpressions, typeof(string), expr);

        //var resolvedValue = sourceExpr.Compile().DynamicInvoke(invocation.Arguments);
        //return resolvedValue!.ToString() ?? string.Empty;

        // 允许字符串内插
        return Smart.Format(attribute.ResourceId, invocation.ArgumentsDictionary);
    }
}