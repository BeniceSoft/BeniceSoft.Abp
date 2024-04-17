using BeniceSoft.Abp.AspNetCore.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace BeniceSoft.Abp.AspNetCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJsonFormatResponse(this IServiceCollection services)
    {
        services.Configure<MvcOptions>(options => { options.Filters.Add<JsonFormatResponseFilter>(); });

        return services;
    }
    
    public static IServiceCollection AddDesensitizeResponse(this IServiceCollection services)
    {
        services.Configure<MvcOptions>(options => { options.Filters.Add<DesensitizeResponseFilter>(); });

        return services;
    }
}