using BeniceSoft.Abp.OperationLogging.Abstractions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace BeniceSoft.Abp.OperationLogging.EventBus;

public class EventBusOperationLogEventDispatcher : IOperationLogEventDispatcher, ITransientDependency
{
    private readonly IDistributedEventBus _eventBus;

    public EventBusOperationLogEventDispatcher(IDistributedEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task DispatchAsync(OperationLogInfo operationLogInfo)
    {
        var @event = new OperationLogEvent()
        {
            ServiceName = operationLogInfo.ServiceName,
            OperationType = operationLogInfo.OperationType,
            BizModule = operationLogInfo.BizModule,
            BizId = operationLogInfo.BizId,
            BizCode = operationLogInfo.BizCode,
            OperatorId = operationLogInfo.OperatorId,
            OperatorName = operationLogInfo.OperatorName,
            OperationTime = operationLogInfo.OperationTime,
            Remark = operationLogInfo.Remark,
            ExtraData = operationLogInfo.ExtraData
        };

        await _eventBus.PublishAsync(@event);
    }
}