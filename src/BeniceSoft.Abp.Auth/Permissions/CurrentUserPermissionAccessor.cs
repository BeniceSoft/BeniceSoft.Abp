using BeniceSoft.Abp.Auth.Core;
using Microsoft.Extensions.Logging;

namespace BeniceSoft.Abp.Auth.Permissions;

public class CurrentUserPermissionAccessor : ICurrentUserPermissionAccessor
{
    private readonly ILogger<CurrentUserPermissionAccessor> _logger;

    private static readonly AsyncLocal<UserPermissionHolder> Holder = new();

    public CurrentUserPermissionAccessor(ILogger<CurrentUserPermissionAccessor> logger)
    {
        _logger = logger;
    }

    public IUserPermission? UserPermission
    {
        get
        {
            var userPermission = Holder.Value?.UserPermission;
            if (!(userPermission?.IsInitialized ?? false))
            {
                _logger.LogWarning("UserPermission 未初始化，使用 app.UseBeniceSoftUserPermission()");
            }

            return userPermission;
        }

        set
        {
            var holder = Holder.Value;
            if (holder != null)
                holder.UserPermission = null!;
            if (value is null)
                return;
            Holder.Value = new()
            {
                UserPermission = value
            };
        }
    }

    class UserPermissionHolder
    {
        public IUserPermission UserPermission;
    }
}