using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Threading;

namespace BeniceSoft.Abp.DynamicPermission;

public class BeniceSoftDynamicPermissionDefinitionInitializer
{
    public static void Initialize(IRootServiceProvider serviceProvider, CancellationTokenSource cancellationTokenSource)
    {
        using var scope = serviceProvider.CreateScope();
        var applicationLifetime = scope.ServiceProvider.GetService<IHostApplicationLifetime>();
        var cancellationTokenProvider = scope.ServiceProvider.GetRequiredService<ICancellationTokenProvider>();
        var cancellationToken = applicationLifetime?.ApplicationStopping ?? cancellationTokenSource.Token;
        var endpoints = scope.ServiceProvider.GetRequiredService<EndpointDataSource>().Endpoints;
        var policies = endpoints
            .Where(x => x is RouteEndpoint && x.Metadata.GetMetadata<IAuthorizeData>() is not null)
            .Select(x => x.Metadata.GetMetadata<IAuthorizeData>().Policy)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        if (policies.Any() == false) return;

        var cache = scope.ServiceProvider.GetRequiredService<DynamicPermissionDefinitionStoreInMemoryCache>();

        try
        {
            using (cancellationTokenProvider.Use(cancellationToken))
            {
                if (cancellationTokenProvider.Token.IsCancellationRequested) return;

                cache.Initialize(policies!);
            }
        }
        // ReSharper disable once EmptyGeneralCatchClause (No need to log since it is logged above)
        catch
        {
        }
    }
}