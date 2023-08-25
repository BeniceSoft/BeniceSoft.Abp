using System;
using Volo.Abp;

namespace BeniceSoft.Abp.Core.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class FillBasicDataValueAttribute : Attribute
{
    /// <summary>
    /// Id 属性名称
    /// </summary>
    public string IdPropertyName { get; set; }

    /// <summary>
    /// 字典根节点KEY值
    /// </summary>
    public string BasicDataRootKey { get; set; }
    
    /// <summary>
    /// 拼接附加属性 例如 Att1、Att2、Att3、Att4
    /// </summary>
    public string[] AppendAttachedProperties { get; set; }

    /// <summary>
    /// 拼接的中间字符
    /// </summary>
    public string JoinAs { get; set; } = string.Empty;
    
    public FillBasicDataValueAttribute(string basicDataRootKey, string idPropertyName) : this(basicDataRootKey, idPropertyName, null)
    {
    }

    public FillBasicDataValueAttribute(string basicDataRootKey, string idPropertyName, params string[] appendAttachedProperties)
    {
        Check.NotNullOrWhiteSpace(basicDataRootKey, nameof(basicDataRootKey));
        Check.NotNullOrWhiteSpace(idPropertyName, nameof(idPropertyName));

        BasicDataRootKey = basicDataRootKey;
        IdPropertyName = idPropertyName;
        AppendAttachedProperties = appendAttachedProperties;
    }
}