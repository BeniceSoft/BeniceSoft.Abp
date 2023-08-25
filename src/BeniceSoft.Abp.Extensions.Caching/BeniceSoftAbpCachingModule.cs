using BeniceSoft.Abp.Extensions.Caching.Configurations;
using BeniceSoft.Abp.Extensions.Caching.Interceptors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.Extensions.Caching;

[DependsOn(
    typeof(AbpCachingModule))]
public class BeniceSoftAbpCachingModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.OnRegistred(CacheableInterceptorRegistrar.RegisterIfNeeded);

        context.Services.GetConfiguration().GetSection("BeniceSoft:Caching").Bind(BeniceSoftCachingConfiguration.Instance);
    }
}