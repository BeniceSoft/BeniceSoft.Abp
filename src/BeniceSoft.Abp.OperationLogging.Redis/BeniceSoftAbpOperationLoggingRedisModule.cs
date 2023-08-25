using BeniceSoft.Abp.OperationLogging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.OperationLogging.Redis;

[DependsOn(
    typeof(AbpCachingStackExchangeRedisModule)
)]
public class BeniceSoftAbpOperationLoggingRedisModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Replace(ServiceDescriptor.Transient<IOperationLogEventDispatcher, RedisOperationLogEventDispatcher>());
    }
}