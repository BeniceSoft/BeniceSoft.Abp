using BeniceSoft.Abp.Extensions.DistributedLock.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.Extensions.DistributedLock;

public class BeniceSoftAbpDistributedLockModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.OnRegistred(DistributedLockInterceptorRegistrar.RegisterIfNeeded);
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var section = context.Services.GetConfiguration().GetSection("UfxDistributedLock");
        context.Services.Configure<DistributedLockOptions>(section);
    }
}