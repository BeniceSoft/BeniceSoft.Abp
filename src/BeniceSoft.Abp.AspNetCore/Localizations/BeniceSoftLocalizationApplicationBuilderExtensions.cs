using System;
using Microsoft.AspNetCore.Builder;

namespace BeniceSoft.Abp.AspNetCore.Localizations;

public static class BeniceSoftLocalizationApplicationBuilderExtensions
{
    public static IApplicationBuilder UseBeniceSoftRequestLocalization(this IApplicationBuilder app,
        Action<RequestLocalizationOptions>? optionsAction = null)
    {
        return app.UseAbpRequestLocalization(options =>
        {
            options.RequestCultureProviders.Insert(0, new BeniceSoftCookieRequestCultureProvider());
            optionsAction?.Invoke(options);
        });
    }
}