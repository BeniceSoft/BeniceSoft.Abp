using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.Ddd.Application.Contracts;

[DependsOn(
    typeof(AbpDddApplicationContractsModule)
)]
public class BeniceSoftAbpDddApplicationContractsModule : AbpModule
{
}