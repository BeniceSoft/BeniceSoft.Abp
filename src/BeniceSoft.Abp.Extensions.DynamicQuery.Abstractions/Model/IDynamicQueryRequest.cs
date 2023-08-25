namespace BeniceSoft.Abp.Extensions.DynamicQuery.Abstractions.Model;

public interface IDynamicQueryRequest
{
    List<DynamicQueryConditionGroup>? ConditionGroups { get; set; }
}