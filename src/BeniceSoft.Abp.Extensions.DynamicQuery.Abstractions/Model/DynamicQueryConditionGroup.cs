namespace BeniceSoft.Abp.Extensions.DynamicQuery.Abstractions.Model;

public class DynamicQueryConditionGroup
{
    /// <summary>
    /// 关系：and，or
    /// </summary>
    public string Relation { get; set; } = DynamicQueryConstants.Relations.And;

    /// <summary>
    /// 条件
    /// </summary>
    public List<DynamicQueryCondition> Conditions { get; set; } = new();
}