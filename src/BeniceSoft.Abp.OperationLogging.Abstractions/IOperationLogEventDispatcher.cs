namespace BeniceSoft.Abp.OperationLogging.Abstractions;

/// <summary>
/// 操作日志事件分发
/// </summary>
public interface IOperationLogEventDispatcher
{
    Task DispatchAsync(OperationLogInfo operationLogInfo);
}