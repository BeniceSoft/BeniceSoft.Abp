namespace BeniceSoft.Abp.Auth.Core.Models;

public class RowPermissionConditionGroup
{
    /// <summary>
    /// 组与组之间的逻辑操作符
    /// and or
    /// </summary>
    public string LogicalOperator { get; set; }

    /// <summary>
    /// 条件
    /// </summary>
    public List<RowPermissionCondition> Conditions { get; set; }
}