using System;
using System.Collections.Generic;
using System.Text;

namespace BeniceSoft.Abp.Auth.Core.Enums;

public enum ExprOperator : int
{
    /// <summary>
    /// 无操作
    /// </summary>
    None = 0,

    /// <summary>
    /// 等于
    /// </summary>
    Equal = 1,

    /// <summary>
    /// 不等于
    /// </summary>
    NotEqual = 2,

    /// <summary>
    /// 大于
    /// </summary>
    GreaterThan = 3,

    /// <summary>
    /// 大于等于
    /// </summary>
    GreaterThanOrEqual = 4,

    /// <summary>
    /// 小于
    /// </summary>
    LessThan = 5,

    /// <summary>
    /// 小于等于
    /// </summary>
    LessThanOrEqual = 6,

    /// <summary>
    /// 包含
    /// </summary>
    Contains = 7,

    /// <summary>
    /// 不包含
    /// </summary>
    NotContains = 8,

    /// <summary>
    /// 开头几位包含
    /// </summary>
    StartsWith = 9,

    /// <summary>
    /// 开头几位不包含
    /// </summary>
    NotStartsWith = 10,

    /// <summary>
    /// 结束几位包含
    /// </summary>
    EndsWith = 11,

    /// <summary>
    /// 结束几位不包含
    /// </summary>
    NotEndsWith = 12,

    /// <summary>
    /// In
    /// </summary>
    In = 13,

    /// <summary>
    /// NotIn
    /// </summary>
    NotIn = 14
}
