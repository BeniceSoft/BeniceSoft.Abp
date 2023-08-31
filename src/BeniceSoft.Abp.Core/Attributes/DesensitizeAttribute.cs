using System;

namespace BeniceSoft.Abp.Core.Attributes;

/// <summary>
/// 数据脱敏
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DesensitizeAttribute : Attribute
{
    /// <summary>
    /// 脱敏类型
    /// </summary>
    public DesensitizeType Type { get; set; }

    /// <summary>
    /// 开始位置（包含）
    /// </summary>
    public int StartIndex { get; set; }

    /// <summary>
    /// 长度
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// 掩码
    /// </summary>
    public char Mask { get; set; } = '*';
}

public enum DesensitizeType : byte
{
    /// <summary>
    /// 自定义
    /// </summary>
    Custom = 0x00,

    /// <summary>
    /// 车牌号
    /// </summary>
    PhoneNumber = 0x01,

    // /// <summary>
    // /// 车牌号
    // /// </summary>
    // CarNumber = 0x02,
    //
    // /// <summary>
    // /// 身份证号
    // /// </summary>
    // IdNumber = 0x03,
    //
    // /// <summary>
    // /// 银行卡号
    // /// </summary>
    // BankCardNumber = 0x04,
}