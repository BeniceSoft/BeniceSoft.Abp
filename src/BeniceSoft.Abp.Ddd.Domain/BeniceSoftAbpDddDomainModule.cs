using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.Ddd.Domain;

[DependsOn(
    typeof(AbpDddDomainModule)
)]
public class BeniceSoftAbpDddDomainModule : AbpModule
{
}