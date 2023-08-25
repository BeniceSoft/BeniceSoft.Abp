using BeniceSoft.Abp.Ddd.Domain;
using BeniceSoft.Abp.Sample.Domain.Shared;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.Sample.Domain;

[DependsOn(
    typeof(BeniceSoftAbpDddDomainModule),
    typeof(SampleDomainSharedModule)
)]
public class SampleDomainModule : AbpModule
{
}