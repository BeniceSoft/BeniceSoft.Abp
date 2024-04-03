using System.Text.Json;
using BeniceSoft.Abp.OperationLogging.Abstractions;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.OperationLogging.Redis;

public class RedisOperationLogEventDispatcher : IOperationLogEventDispatcher, ITransientDependency
{
    private readonly ConnectionMultiplexer _multiplexer;
    private readonly OperationLoggingRedisOptions _options;

    public RedisOperationLogEventDispatcher(ConnectionMultiplexer multiplexer, IOptions<OperationLoggingRedisOptions> options)
    {
        _multiplexer = multiplexer;
        _options = options.Value;
    }

    public async Task DispatchAsync(OperationLogInfo operationLogInfo)
    {
        var subscriber = _multiplexer.GetSubscriber();
        using var memory = new MemoryStream();
        await JsonSerializer.SerializeAsync(memory, operationLogInfo);
        await subscriber.PublishAsync(_options.Channel, memory.GetBuffer());
    }
}