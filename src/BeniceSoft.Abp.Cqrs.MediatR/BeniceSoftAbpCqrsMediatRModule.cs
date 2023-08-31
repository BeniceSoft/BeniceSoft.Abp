using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.Cqrs.MediatR;

[DependsOn(
    typeof(BeniceSoftAbpCqrsModule)
)]
public class BeniceSoftAbpCqrsMediatRModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        
    }
}