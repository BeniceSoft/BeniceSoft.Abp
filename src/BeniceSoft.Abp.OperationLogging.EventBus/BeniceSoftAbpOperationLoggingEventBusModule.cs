using BeniceSoft.Abp.OperationLogging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.EventBus;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.OperationLogging.EventBus;

[DependsOn(
    typeof(AbpEventBusModule)
)]
public class BeniceSoftAbpOperationLoggingEventBusModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Replace(ServiceDescriptor.Transient<IOperationLogEventDispatcher, EventBusOperationLogEventDispatcher>());
    }
}