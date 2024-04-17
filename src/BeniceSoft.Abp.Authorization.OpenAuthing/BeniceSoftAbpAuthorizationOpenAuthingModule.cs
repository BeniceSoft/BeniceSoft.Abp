using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.Authorization.OpenAuthing;

[DependsOn(
    typeof(BeniceSoftAbpAuthorizationModule)
)]
public class BeniceSoftAbpAuthorizationOpenAuthingModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpPermissionOptions>(options =>
        {
            options.ValueProviders.AddIfNotContains(typeof(OpenAuthingPermissionValueProvider));
        });
    }
}