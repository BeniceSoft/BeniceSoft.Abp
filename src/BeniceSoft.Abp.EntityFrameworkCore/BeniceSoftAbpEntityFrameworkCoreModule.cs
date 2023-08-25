using BeniceSoft.Abp.Ddd.Domain;
using BeniceSoft.Abp.Extensions.DynamicQuery.EfCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.EntityFrameworkCore;

[DependsOn(
    typeof(AbpEntityFrameworkCoreModule),
    typeof(BeniceSoftAbpDddDomainModule),
    typeof(BeniceSoftAbpDynamicQueryEfCoreModule)
)]
public class BeniceSoftAbpEntityFrameworkCoreModule : AbpModule
{
}