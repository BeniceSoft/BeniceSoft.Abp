namespace BeniceSoft.Abp.Auth.Core.Models;

public class RowPermission
{
    /// <summary>
    /// 表名
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    /// 条件
    /// </summary>
    public List<RowPermissionConditionGroup> ConditionGroups { get; set; }
}