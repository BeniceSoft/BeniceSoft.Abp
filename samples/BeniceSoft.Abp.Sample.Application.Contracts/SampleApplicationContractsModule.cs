using BeniceSoft.Abp.Ddd.Application.Contracts;
using BeniceSoft.Abp.Sample.Domain.Shared;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.Sample.Application.Contracts;

[DependsOn(
    typeof(BeniceSoftAbpDddApplicationContractsModule),
    typeof(SampleDomainSharedModule)
)]
public class SampleApplicationContractsModule : AbpModule
{
}