using BeniceSoft.Abp.OperationLogging.Abstractions;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.OperationLogging;

public class NullOperationLogEventDispatcher : IOperationLogEventDispatcher, ITransientDependency
{
    private readonly ILogger<NullOperationLogEventDispatcher> _logger;

    public NullOperationLogEventDispatcher(ILogger<NullOperationLogEventDispatcher> logger)
    {
        _logger = logger;
    }

    public Task DispatchAsync(OperationLogInfo operationLogInfo)
    {
        _logger.LogInformation("Operation log: {0} {1} at {2}", operationLogInfo.OperatorName, operationLogInfo.OperationType, operationLogInfo.OperationTime);
        return Task.CompletedTask;
    }
}