namespace BeniceSoft.Abp.OperationLogging.Abstractions;

public class OperationLogContext
{
    /// <summary>
    /// 业务id
    /// </summary>
    public string? BizId { get; set; }

    /// <summary>
    /// 业务编码
    /// </summary>
    public string? BizCode { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 扩展数据
    /// </summary>
    public Dictionary<string, object> ExtraData { get; set; } = new();

    public void SetValue(string bizId, string bizCode, string remark, Dictionary<string, object> extraData)
    {
        BizId = bizId;
        BizCode = bizCode;
        Remark = remark;
        ExtraData = extraData;
    }
}