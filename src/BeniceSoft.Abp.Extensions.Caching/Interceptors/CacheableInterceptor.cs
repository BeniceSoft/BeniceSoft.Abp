using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using BeniceSoft.Abp.Extensions.Caching.Internals;
using BeniceSoft.Abp.Extensions.Caching.Abstractions.Annotations;
using BeniceSoft.Abp.Extensions.Caching.Abstractions.Interfaces;
using BeniceSoft.Abp.Extensions.Caching.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;

namespace BeniceSoft.Abp.Extensions.Caching.Interceptors;

/// <summary>
/// Cacheable 拦截器
/// </summary>
public class CacheableInterceptor : AbpInterceptor, ITransientDependency
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheableInterceptor> _logger;
    private readonly ICacheValueSerializer _valueSerializer;

    public CacheableInterceptor(ILogger<CacheableInterceptor> logger, IDistributedCache cache, ICacheValueSerializer valueSerializer)
    {
        _logger = logger;
        _cache = cache;
        _valueSerializer = valueSerializer;
    }

    public override async Task InterceptAsync(IAbpMethodInvocation invocation)
    {
        if (CanCacheable(invocation, out var cacheableAttribute))
        {
            var targetMethod = invocation.Method;
            var methodReturnType = targetMethod.ReturnType;
            var methodName = GetMethodFullName(targetMethod);

            // 如果方法有返回值
            if (!methodReturnType.IsVoidType())
            {
                // 同步方法不支持
                if (!invocation.IsAsync())
                {
                    // https://github.com/abpframework/abp/pull/9164 同步方法不调用 await invocation.ProceedAsync() 将不起作用
                    _logger.LogWarning("[Cacheable] 仅可使用在异步方法中");
                    await invocation.ProceedAsync();
                    return;
                }

                var key = GenerateCacheKey(invocation, cacheableAttribute!);

                var returnType = methodReturnType;
                // 如果是 Task<T1> or ValueTask<T1> 那么取 T1
                if (invocation.IsAsync())
                {
                    returnType = targetMethod.ReturnType.GetGenericArguments().First();
                }

                // 尝试从缓存中获取值 获取到直接赋值给方法返回值
                var value = await TryGetValueFromCacheAsync(key, returnType);
                if (value is not null)
                {
                    _logger.LogDebug("方法{0}已从缓存中获取到返回值{1}", methodName, value);

                    invocation.ReturnValue = WarpReturnValue(value, methodReturnType, returnType);
                    return;
                }

                // 缓存中没有值 先执行方法
                await invocation.ProceedAsync();

                // 判断是否需要缓存值
                if (!await IsUnlessAsync(invocation, cacheableAttribute!, returnType))
                {
                    // 将返回值缓存
                    await TrySetValueToCacheAsync(key, invocation, cacheableAttribute!);
                    _logger.LogDebug("已将方法{0}返回值缓存，{1}", methodName, key);

                    return;
                }

                _logger.LogDebug("命中 Unless {0}，返回值不缓存", cacheableAttribute!.Unless);
                return;
            }

            _logger.LogDebug("方法{0}没有返回值，跳过缓存", methodName);
        }

        await invocation.ProceedAsync();
    }

    /// <summary>
    /// 是否可缓存
    /// </summary>
    /// <param name="invocation"></param>
    /// <param name="cacheableAttribute"></param>
    /// <returns></returns>
    private bool CanCacheable(IAbpMethodInvocation invocation, out CacheableAttribute? cacheableAttribute)
    {
        // 优先从方法中取
        // 方法上没有则从类上取
        cacheableAttribute = invocation.Method.GetCustomAttribute<CacheableAttribute>() ??
                             invocation.Method.DeclaringType?.GetCustomAttribute<CacheableAttribute>();

        // 不存在 Cacheable Attribute 不可缓存
        if (cacheableAttribute is null) return false;

        // 如果设置了缓存条件
        if (!string.IsNullOrWhiteSpace(cacheableAttribute.Condition))
        {
            var parameters = BuildArgumentParameters(invocation);

            var lambda = DynamicExpressionParser.ParseLambda(parameters.ToArray(), typeof(bool), cacheableAttribute.Condition);
            var matched = (bool?)lambda.Compile().DynamicInvoke(invocation.Arguments) ?? false;
            if (!matched)
            {
                _logger.LogDebug("方法{0}未满足条件{1}，跳过缓存",
                    GetMethodFullName(invocation.Method),
                    cacheableAttribute.Condition);
            }

            return matched;
        }

        return true;
    }

    /// <summary>
    /// 是否需要将返回值缓存
    /// </summary>
    /// <param name="invocation"></param>
    /// <param name="cacheableAttribute"></param>
    /// <param name="returnType"></param>
    /// <returns></returns>
    private async Task<bool> IsUnlessAsync(IAbpMethodInvocation invocation, CacheableAttribute cacheableAttribute, Type returnType)
    {
        if (!string.IsNullOrWhiteSpace(cacheableAttribute.Unless))
        {
            var parameters = BuildArgumentParameters(invocation);
            var args = invocation.Arguments?.ToList() ?? new();
            if (parameters.All(x => x.Name != "@result"))
            {
                // TODO 如果方法有同名的 @result 参数怎么办
                parameters.Add(Expression.Parameter(returnType, "@result"));
                args.Add(await GetReturnValueAsync(invocation));
            }

            var lambda = DynamicExpressionParser.ParseLambda(parameters.ToArray(), typeof(bool), cacheableAttribute.Unless);
            var isUnless = (bool?)lambda.Compile().DynamicInvoke(args.ToArray());

            return isUnless ?? false;
        }

        return false;
    }

    /// <summary>
    /// 包装返回值
    /// </summary>
    /// <param name="value"></param>
    /// <param name="methodReturnType"></param>
    /// <param name="returnType"></param>
    /// <returns></returns>
    private object? WarpReturnValue(object? value, Type methodReturnType, Type returnType)
    {
        if (value is null) return default;

        if (methodReturnType == typeof(ValueTask<>).MakeGenericType(returnType))
        {
            // TODO ValueTask<> 会抛异常 Interceptors failed to set a return value, or swallowed the exception thrown by the target
            value = Activator.CreateInstance(typeof(ValueTask<>).MakeGenericType(returnType), value);
        }

        return value;
    }

    /// <summary>
    /// 获取方法完整名称
    /// </summary>
    /// <param name="methodInfo"></param>
    /// <returns></returns>
    private string GetMethodFullName(MethodInfo methodInfo)
    {
        // TODO 加上方法参数
        return $"{methodInfo.DeclaringType?.Namespace}.{methodInfo.DeclaringType?.Name}.{methodInfo.Name}";
    }

    /// <summary>
    /// 生成缓存键
    /// </summary>
    /// <param name="invocation"></param>
    /// <param name="attribute"></param>
    /// <returns></returns>
    private string GenerateCacheKey(IAbpMethodInvocation invocation, CacheableAttribute attribute)
    {
        ICacheKeyGenerator? keyGenerator = null;
        if (attribute.KeyGeneratorType is not null)
        {
            if (attribute.KeyGeneratorType != typeof(ICacheKeyGenerator) ||
                attribute.KeyGeneratorType.IsInterface ||
                attribute.KeyGeneratorType.IsAbstract)
                throw new AbpException("KeyGeneratorType 需要为实现 ICacheKeyGenerator 可实例化的类");
            var constructorInfo = attribute.KeyGeneratorType.GetConstructor(new Type[] { });
            if (constructorInfo is null)
                throw new AbpException("KeyGeneratorType 需要无参构造函数");
            keyGenerator = constructorInfo.Invoke(null) as ICacheKeyGenerator;
        }

        keyGenerator ??= new SimpleCacheKeyGenerator();
        return keyGenerator.Generate(invocation, attribute.Key);
    }

    /// <summary>
    /// 尝试从缓存中获取值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="returnType"></param>
    /// <returns></returns>
    private async Task<object?> TryGetValueFromCacheAsync(string key, Type returnType)
    {
        try
        {
            var bytes = await _cache.GetAsync(key);
            if (bytes is null) return null;

            return _valueSerializer.Deserialize(bytes, returnType);
        }
        catch (Exception e)
        {
            _logger.LogWarning("获取方法缓存失败：{0}", e.Message);
        }

        return default;
    }

    /// <summary>
    /// 尝试将值写入缓存中
    /// </summary>
    /// <param name="key"></param>
    /// <param name="invocation"></param>
    /// <param name="cacheableAttribute"></param>
    /// type
    /// <returns></returns>
    private async Task TrySetValueToCacheAsync(string key, IAbpMethodInvocation invocation, CacheableAttribute cacheableAttribute)
    {
        try
        {
            var returnValue = await GetReturnValueAsync(invocation);

            var bytes = _valueSerializer.Serialize(returnValue);
            var cacheOptions = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = cacheableAttribute.GetExpireOn()
            };
            await _cache.SetAsync(key, bytes, cacheOptions);
        }
        catch (Exception e)
        {
            _logger.LogWarning("设置方法缓存失败：{0}", e.Message);
        }
    }

    /// <summary>
    /// 获取方法返回值
    /// </summary>
    /// <param name="invocation"></param>
    /// <returns></returns>
    private async Task<object?> GetReturnValueAsync(IAbpMethodInvocation invocation)
    {
        var returnValue = invocation.ReturnValue;
        if (invocation.IsAsync())
        {
            returnValue = await invocation.UnwrapAsyncReturnValue();
        }

        return returnValue;
    }

    /// <summary>
    /// 将方法参数构建成参数表达式
    /// </summary>
    /// <param name="invocation"></param>
    /// <returns></returns>
    private List<ParameterExpression> BuildArgumentParameters(IAbpMethodInvocation invocation)
    {
        var methodParameters = invocation.Method.GetParameters()
            .ToDictionary(p => p.Name!, p => p.ParameterType);
        return invocation
            .ArgumentsDictionary
            .Select(x => Expression.Parameter(methodParameters[x.Key], x.Key))
            .ToList();
    }
}