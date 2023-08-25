using BeniceSoft.Abp.OperationLogging.Abstractions;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.OperationLogging;

public class NullOperationLogEventDispatcher : IOperationLogEventDispatcher, ITransientDependency
{
    public Task DispatchAsync(OperationLogInfo operationLogInfo)
    {
        return Task.CompletedTask;
    }
}