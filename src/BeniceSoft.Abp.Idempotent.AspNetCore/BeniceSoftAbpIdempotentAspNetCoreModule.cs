using BeniceSoft.Abp.Idempotent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.Idempotent.AspNetCore;

[DependsOn(
    typeof(BeniceSoftAbpIdempotentModule)
)]
public class BeniceSoftAbpIdempotentAspNetCoreModule : AbpModule
{

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Configure<MvcOptions>(x =>
        {
            x.Filters.Add<IdempotencyFilter>();
        });
        var idempotencyOptions = context.Services.ExecutePreConfiguredActions<IdempotencyOptions>();
    }
}