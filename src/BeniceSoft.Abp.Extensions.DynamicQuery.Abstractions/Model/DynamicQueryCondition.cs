namespace BeniceSoft.Abp.Extensions.DynamicQuery.Abstractions.Model;

public class DynamicQueryCondition
{
    /// <summary>
    /// 关系：and，or
    /// </summary>
    public string Relation { get; set; } = DynamicQueryConstants.Relations.And;

    /// <summary>
    /// 字段名
    /// </summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// 字段类型，支持： "integer", "double", "string", "date", "datetime", and "boolean".
    /// </summary>
    public string FieldType { get; set; } = string.Empty;

    /// <summary>
    /// 操作符
    /// </summary>
    public string Operator { get; set; } = string.Empty;

    /// <summary>
    /// 比较值
    /// </summary>
    public List<string> Value { get; set; } = new();
}