using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using BeniceSoft.Abp.Extensions.Caching.Configurations;
using BeniceSoft.Abp.Extensions.Caching.Abstractions.Interfaces;
using Volo.Abp.DynamicProxy;

namespace BeniceSoft.Abp.Extensions.Caching.Internals;

internal class SimpleCacheKeyGenerator : ICacheKeyGenerator
{
    public string Generate(IAbpMethodInvocation invocation, string? key)
    {
        var methodInfo = invocation.Method;
        var cacheKey = new StringBuilder();
        if (!BeniceSoftCachingConfiguration.Instance.CacheKeyPrefix.IsNullOrWhiteSpace())
        {
            cacheKey.Append(BeniceSoftCachingConfiguration.Instance.CacheKeyPrefix);
        }

        cacheKey.AppendJoin(".",
            methodInfo.DeclaringType?.Namespace,
            methodInfo.DeclaringType?.Name,
            methodInfo.Name);

        var methodParameters = methodInfo.GetParameters();

        // 如果未设置缓存键 则使用方法参数
        if (string.IsNullOrWhiteSpace(key))
        {
            if (methodParameters.Any())
            {
                cacheKey.Append(":");
                cacheKey.AppendJoin("_", methodParameters.Select(x => $"{x.ParameterType.Name}_{x.Name}"));
            }
        }
        else
        {
            cacheKey.Append(":");

            var parameters = invocation
                .ArgumentsDictionary
                .Select(x => Expression.Parameter(methodParameters.Single(p => p.Name == x.Key).ParameterType, x.Key))
                .ToArray();
            var lambda = DynamicExpressionParser.ParseLambda(parameters, typeof(string), key);
            var value = lambda.Compile().DynamicInvoke(invocation.Arguments);
            cacheKey.Append(value);
        }


        return cacheKey.ToString();
    }
}