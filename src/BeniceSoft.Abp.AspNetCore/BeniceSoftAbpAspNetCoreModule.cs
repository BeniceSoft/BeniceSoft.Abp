using System.Collections.Generic;
using System.Linq;
using BeniceSoft.Abp.AspNetCore.Filters;
using BeniceSoft.Abp.AspNetCore.Localizations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.ExceptionHandling;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.AspNetCore;

[DependsOn(
    typeof(AbpAspNetCoreMvcModule)
)]
public class BeniceSoftAbpAspNetCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<MvcOptions>(options =>
        {
            // 将 abp 的错误拦截器移除
            var filterMetadata = options.Filters.FirstOrDefault(x =>
                x is ServiceFilterAttribute attribute && attribute.ServiceType == typeof(AbpExceptionFilter));
            if (filterMetadata is not null)
            {
                options.Filters.Remove(filterMetadata);
            }
        });

        Configure<BeniceSoftCultureMapOptions>(options =>
        {
            var zhHansCultureMapInfo = new CultureMapInfo
            {
                TargetCulture = "zh-Hans",
                SourceCultures = new List<string>
                {
                    "zh", "zh_cn", "zh-CN"
                }
            };
            options.CulturesMaps.Add(zhHansCultureMapInfo);
            options.UiCulturesMaps.Add(zhHansCultureMapInfo);

            var enUsCultureMapInfo = new CultureMapInfo
            {
                TargetCulture = "en-US",
                SourceCultures = new List<string>
                {
                    "en", "en_us", "en-US"
                }
            };
            options.CulturesMaps.Add(enUsCultureMapInfo);
            options.UiCulturesMaps.Add(enUsCultureMapInfo);
        });
    }
}