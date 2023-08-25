using System.Reflection;
using BeniceSoft.Abp.Core.Attributes;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Conventions;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.AspNetCore.Conversions;

[ExposeServices(typeof(IAbpServiceConvention))]
public class BeniceSoftAbpServiceConvention : AbpServiceConvention
{
    public BeniceSoftAbpServiceConvention(IOptions<AbpAspNetCoreMvcOptions> options, IConventionalRouteBuilder conventionalRouteBuilder) : base(options,
        conventionalRouteBuilder)
    {
    }

    protected override void ConfigureParameters(ControllerModel controller)
    {
        foreach (var action in controller.Actions)
        {
            var parameters = action.Parameters;
            foreach (var prm in parameters)
            {
                if (prm.ParameterInfo.GetCustomAttribute<IgnoreBindAttribute>() is not null)
                {
                    action.Parameters.Remove(prm);
                    break;
                }
            }
        }

        base.ConfigureParameters(controller);
    }
}