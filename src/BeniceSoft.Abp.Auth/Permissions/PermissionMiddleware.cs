using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;

namespace BeniceSoft.Abp.Auth.Permissions;

public class PermissionMiddleware : IMiddleware, ITransientDependency
{
    private readonly IUserPermissionFactory _userPermissionFactory;
    private readonly ILogger<PermissionMiddleware> _logger;
    private readonly ICurrentUser _currentUser;

    public PermissionMiddleware(IUserPermissionFactory userPermissionFactory, ILogger<PermissionMiddleware> logger,
        ICurrentUser currentUser)
    {
        _userPermissionFactory = userPermissionFactory;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            if (_currentUser.IsAuthenticated)
            {
                var userId = _currentUser.Id!.Value;

                _logger.LogInformation("Initialize user {0} permissions", userId);

                var userPermission = await _userPermissionFactory.CreateAsync(userId, context);

                context.Features.Set(userPermission);
            }

            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, e.Message);
            throw;
        }
        finally
        {
            _userPermissionFactory.Dispose();
        }
    }
}