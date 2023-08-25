using System.Text.Json;
using BeniceSoft.Abp.OperationLogging.Abstractions;
using StackExchange.Redis;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.OperationLogging.Redis;

public class RedisOperationLogEventDispatcher : IOperationLogEventDispatcher, ITransientDependency
{
    private readonly ConnectionMultiplexer _multiplexer;

    public RedisOperationLogEventDispatcher(ConnectionMultiplexer multiplexer)
    {
        _multiplexer = multiplexer;
    }

    public async Task DispatchAsync(OperationLogInfo operationLogInfo)
    {
        var subscriber = _multiplexer.GetSubscriber();
        using var memory = new MemoryStream();
        await JsonSerializer.SerializeAsync(memory, operationLogInfo);
        await subscriber.PublishAsync("", memory.GetBuffer());
    }
}