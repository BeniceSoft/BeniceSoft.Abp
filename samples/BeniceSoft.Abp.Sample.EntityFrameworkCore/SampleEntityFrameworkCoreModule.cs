using BeniceSoft.Abp.EntityFrameworkCore;
using BeniceSoft.Abp.Sample.Domain;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.Sample.EntityFrameworkCore;

[DependsOn(
    typeof(BeniceSoftAbpEntityFrameworkCoreModule),
    typeof(SampleDomainModule)
)]
public class SampleEntityFrameworkCoreModule : AbpModule
{
}