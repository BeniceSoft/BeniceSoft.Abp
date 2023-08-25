using BeniceSoft.Abp.Auth.Core.Models;

namespace BeniceSoft.Abp.Auth.Core;

/// <summary>
/// 当前用户权限：数据权限（行、列）；功能权限（暂无）
/// </summary>
public interface IUserPermission
{
    /// <summary>
    /// 是否已初始化
    /// </summary>
    public bool IsInitialized { get; }

    /// <summary>
    /// 用户id
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// 行权限
    /// </summary>
    public List<RowPermission>? RowPermissions { get; }

    /// <summary>
    /// 列权限
    /// </summary>
    public List<ColumnPermission>? ColumnPermissions { get; }
}