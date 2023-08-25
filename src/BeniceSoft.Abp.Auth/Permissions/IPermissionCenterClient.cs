using BeniceSoft.Abp.Auth.Core;
using BeniceSoft.Abp.Auth.Core.Models;

namespace BeniceSoft.Abp.Auth.Permissions;

public interface IPermissionCenterClient
{
    Task<List<RowPermission>?> GetUserRowPermissions(Guid userId, string accessToken);
    Task<List<ColumnPermission>?> GetUserColumnPermissions(Guid userId, string accessToken);
}