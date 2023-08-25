namespace BeniceSoft.Abp.Auth.Core;

/// <summary>
/// 列权限过滤标签
/// 参数格式："表名.列名"
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ColumnAuthDescAttribute : Attribute
{
    /// <summary>
    /// 表名.列名
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// 隐藏后显示的值
    /// </summary>
    public string MaskedValue { get; private set; }

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="Desc">表名.列名</param>
    /// <param name="MaskedValue">隐藏后显示的值</param>
    public ColumnAuthDescAttribute(string Desc, string MaskedValue = "0")
    {
        this.Description = Desc;
        this.MaskedValue = MaskedValue;
    }
}