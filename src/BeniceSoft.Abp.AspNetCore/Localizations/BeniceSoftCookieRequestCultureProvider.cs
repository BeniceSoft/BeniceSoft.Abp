using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.RequestLocalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace BeniceSoft.Abp.AspNetCore.Localizations;

public class BeniceSoftCookieRequestCultureProvider : RequestCultureProvider
{
    private const string CookieName = ".benicesoft.language";

    public override async Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        if (httpContext == null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        var option = httpContext.RequestServices.GetService<IOptions<BeniceSoftCultureMapOptions>>()?.Value;
        if (option is not null)
        {
            var language = httpContext.Request.Cookies[CookieName];
            if (!string.IsNullOrWhiteSpace(language))
            {
                var mapCulture = (StringSegment)(option.CulturesMaps
                    .FirstOrDefault(x => x.SourceCultures.Contains(language, StringComparer.OrdinalIgnoreCase))?.TargetCulture ?? language);
                var mapUiCulture = (StringSegment)(option.UiCulturesMaps
                    .FirstOrDefault(x => x.SourceCultures.Contains(language, StringComparer.OrdinalIgnoreCase))?.TargetCulture ?? language);

                return new(mapCulture, mapUiCulture);
            }
        }

        return await NullProviderCultureResult;
    }
}