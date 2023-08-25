using BeniceSoft.Abp.Auth.Core;
using BeniceSoft.Abp.Auth.Core.Models;

namespace BeniceSoft.Abp.Auth.Permissions;

[Serializable]
public class UserPermission : IUserPermission
{
    public bool IsInitialized { get; set; }
    public Guid UserId { get; set; }
    public List<RowPermission>? RowPermissions { get; set; }
    public List<ColumnPermission>? ColumnPermissions { get; set; }
}