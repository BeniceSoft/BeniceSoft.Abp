using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.Authorization;

[DependsOn(
    typeof(AbpAuthorizationModule)
)]
public class BeniceSoftAbpAuthorizationModule : AbpModule
{
    
}