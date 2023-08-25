namespace BeniceSoft.Abp.OperationLogging.Abstractions;

/// <summary>
/// 操作日志
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class OperationLogAttribute : Attribute
{
    /// <summary>
    /// 操作类型
    /// </summary>
    public string OperationType { get; set; }

    /// <summary>
    /// 业务模块
    /// </summary>
    public string BizModule { get; set; }

    /// <summary>
    /// 业务id
    /// </summary>
    public string BizId { get; set; }
}