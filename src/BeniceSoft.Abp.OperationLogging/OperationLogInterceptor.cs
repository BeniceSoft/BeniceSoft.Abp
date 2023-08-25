using System.Reflection;
using BeniceSoft.Abp.OperationLogging.Abstractions;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;
using Volo.Abp.Timing;
using Volo.Abp.Users;

namespace BeniceSoft.Abp.OperationLogging;

public class OperationLogInterceptor : IAbpInterceptor, ITransientDependency
{
    private readonly IOperationLogEventDispatcher _eventDispatcher;
    private readonly IClock _clock;
    private readonly BeniceSoftOperationLogOptions _options;
    private readonly ICurrentUser _currentUser;

    public OperationLogInterceptor(IOperationLogEventDispatcher eventDispatcher, IClock clock, IOptions<BeniceSoftOperationLogOptions> options,
        ICurrentUser currentUser)
    {
        _eventDispatcher = eventDispatcher;
        _clock = clock;
        _currentUser = currentUser;
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task InterceptAsync(IAbpMethodInvocation invocation)
    {
        var targetMethod = invocation.Method;
        var operationLogAttribute = targetMethod.GetCustomAttribute<OperationLogAttribute>();
        if (operationLogAttribute is null)
        {
            await invocation.ProceedAsync();
            return;
        }

        var parameters = targetMethod.GetParameters();

        var context = new OperationLogContext();

        // 如果方法最后一个参数是 OperationLogContext 类型，那么自动赋值
        if (parameters.LastOrDefault()?.ParameterType == typeof(OperationLogContext))
        {
            invocation.Arguments[^1] = context;
        }

        await invocation.ProceedAsync();

        // 记录操作日志
        var log = new OperationLogInfo
        {
            ServiceName = _options.ServiceName,
            OperationType = operationLogAttribute.OperationType,
            BizModule = operationLogAttribute.BizModule,
            BizId = context.BizId ?? operationLogAttribute.BizId,
            BizCode = context.BizCode ?? string.Empty,
            OperatorId = _currentUser.Id,
            OperatorName = _currentUser.Name,
            OperationTime = _clock.Now,
            Remark = context.Remark,
            ExtraData = context.ExtraData
        };
        await _eventDispatcher.DispatchAsync(log);
    }
}