using BeniceSoft.Abp.Ddd.Application.Contracts;
using BeniceSoft.Abp.Ddd.Domain;
using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.Ddd.Application;

[DependsOn(
    typeof(AbpDddApplicationModule),
    typeof(BeniceSoftAbpDddDomainModule),
    typeof(BeniceSoftAbpDddApplicationContractsModule)
)]
public class BeniceSoftAbpDddApplicationModule : AbpModule
{
}