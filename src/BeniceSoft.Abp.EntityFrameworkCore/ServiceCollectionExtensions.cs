using BeniceSoft.Abp.Ddd.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BeniceSoft.Abp.EntityFrameworkCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEntityTableNameResolver<TContext>(this IServiceCollection services) where TContext : DbContext
    {
        services.TryAddSingleton(typeof(IEntityTableNameResolver), typeof(EntityFrameworkCoreEntityTableNameResolver<TContext>));

        return services;
    }
}