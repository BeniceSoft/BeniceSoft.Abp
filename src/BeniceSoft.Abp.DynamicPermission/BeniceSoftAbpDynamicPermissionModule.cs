using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace BeniceSoft.Abp.DynamicPermission;

[DependsOn(
    typeof(AbpAuthorizationModule)
)]
public class BeniceSoftAbpDynamicPermissionModule : AbpModule
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();


    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(() => OnPostApplicationInitializationAsync(context));
    }

    public override Task OnPostApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        InitializeDynamicPermissions(context);
        return Task.CompletedTask;
    }

    public override Task OnApplicationShutdownAsync(ApplicationShutdownContext context)
    {
        _cancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }

    private void InitializeDynamicPermissions(ApplicationInitializationContext context)
    {
        var rootServiceProvider = context.ServiceProvider.GetRequiredService<IRootServiceProvider>();
        BeniceSoftDynamicPermissionDefinitionInitializer.Initialize(rootServiceProvider, _cancellationTokenSource);
    }
}