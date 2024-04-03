using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.OperationLogging;

public class BeniceSoftAbpOperationLoggingModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.OnRegistered(OperationLogInterceptorRegister.RegisterIfNeeded);
    }
}