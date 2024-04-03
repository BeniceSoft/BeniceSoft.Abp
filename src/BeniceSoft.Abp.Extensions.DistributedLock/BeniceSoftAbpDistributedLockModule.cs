using BeniceSoft.Abp.Extensions.DistributedLock.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.Extensions.DistributedLock;

public class BeniceSoftAbpDistributedLockModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.OnRegistered(DistributedLockInterceptorRegistrar.RegisterIfNeeded);
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var section = context.Services.GetConfiguration().GetSection("BeniceSoft:DistributedLock");
        context.Services.Configure<DistributedLockOptions>(section);
    }
}