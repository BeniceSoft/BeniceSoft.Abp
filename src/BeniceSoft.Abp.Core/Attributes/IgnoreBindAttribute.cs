using System;

namespace BeniceSoft.Abp.Core.Attributes;

/// <summary>
/// 忽略模型绑定
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class IgnoreBindAttribute : Attribute
{
}