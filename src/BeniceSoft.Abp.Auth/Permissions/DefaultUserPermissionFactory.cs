using System.Text.Json;
using BeniceSoft.Abp.Auth.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace BeniceSoft.Abp.Auth.Permissions;

public class DefaultUserPermissionFactory : IUserPermissionFactory
{
    private readonly ICurrentUserPermissionAccessor _userPermissionAccessor;
    private readonly IPermissionCenterClient _permissionCenterClient;
    private readonly ILogger<DefaultUserPermissionFactory> _logger;
    private readonly IDistributedCache _distributedCache;

    public DefaultUserPermissionFactory(ICurrentUserPermissionAccessor userPermissionAccessor, IPermissionCenterClient permissionCenterClient,
        ILogger<DefaultUserPermissionFactory> logger, IDistributedCache distributedCache)
    {
        _userPermissionAccessor = userPermissionAccessor;
        _permissionCenterClient = permissionCenterClient;
        _logger = logger;
        _distributedCache = distributedCache;
    }

    public async Task<IUserPermission> CreateAsync(Guid userId, HttpContext httpContext)
    {
        var userPermission = await GetFromCacheAsync(userId);
        if (userPermission is null)
        {
            var accessToken = httpContext.Request.Headers[HeaderNames.Authorization];

            userPermission = new UserPermission
            {
                IsInitialized = true,
                UserId = userId,
                RowPermissions = await _permissionCenterClient.GetUserRowPermissions(userId, accessToken!),
                ColumnPermissions = await _permissionCenterClient.GetUserColumnPermissions(userId, accessToken!),
            };

            await SetToCacheAsync(userPermission);
        }

        return Initialize(userPermission);
    }


    private IUserPermission Initialize(IUserPermission userPermission)
    {
        _userPermissionAccessor.UserPermission = userPermission;

        _logger.LogInformation("Initialized user {0} permissions.", userPermission.UserId);

        return userPermission;
    }

    public void Dispose()
    {
        _userPermissionAccessor.UserPermission = null;
    }


    private async Task<IUserPermission?> GetFromCacheAsync(Guid userId)
    {
        var key = CacheKey(userId);
        var bytes = await _distributedCache.GetAsync(key);
        if (bytes is null) return default;

        return await JsonSerializer.DeserializeAsync<UserPermission>(new MemoryStream(bytes));
    }

    private async Task SetToCacheAsync(IUserPermission userPermission)
    {
        var key = CacheKey(userPermission.UserId);
        var bytes = JsonSerializer.SerializeToUtf8Bytes(userPermission);
        await _distributedCache.SetAsync(key, bytes);
    }


    private string CacheKey(Guid userId) => $"Ufx:Auth:UserPermissions:{userId}";
}