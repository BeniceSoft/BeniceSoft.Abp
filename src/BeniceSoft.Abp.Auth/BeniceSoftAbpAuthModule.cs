using BeniceSoft.Abp.Auth.Permissions;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.Auth;

[DependsOn(
    typeof(AbpAuthorizationModule)
)]
public class BeniceSoftAbpAuthModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services
            .AddHttpClient(PermissionCenterClient.PermissionCenterHttpClientName);
    }
}