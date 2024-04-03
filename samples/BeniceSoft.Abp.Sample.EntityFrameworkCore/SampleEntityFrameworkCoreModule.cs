using BeniceSoft.Abp.EntityFrameworkCore;
using BeniceSoft.Abp.Sample.Domain;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.Sample.EntityFrameworkCore;

[DependsOn(
    typeof(BeniceSoftAbpEntityFrameworkCoreModule),
    typeof(SampleDomainModule)
)]
public class SampleEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<SampleDbContext>(builder => { builder.AddDefaultRepositories(); });
        context.Services.AddEntityTableNameResolver<SampleDbContext>();
    }
}