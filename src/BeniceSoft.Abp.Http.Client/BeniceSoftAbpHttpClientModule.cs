using BeniceSoft.Abp.Core;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.Http.Client;

[DependsOn(
    typeof(BeniceSoftAbpCoreModule),
    typeof(AbpHttpClientModule)
)]
public class BeniceSoftAbpHttpClientModule : AbpModule
{
}