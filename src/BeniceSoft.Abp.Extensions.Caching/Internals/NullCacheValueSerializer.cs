using BeniceSoft.Abp.Extensions.Caching.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.Extensions.Caching.Internals;

public class NullCacheValueSerializer : ICacheValueSerializer, ISingletonDependency
{
    public NullCacheValueSerializer(ILogger<NullCacheValueSerializer> logger)
    {
        _logger = logger;
    }

    public string Name => "Null";

    private readonly ILogger<NullCacheValueSerializer> _logger;


    public byte[] Serialize<TValue>(TValue data)
    {
        _logger.LogWarning("Serialize is null, use MessagePack or SystemTextJson Serializer");
        return Array.Empty<byte>();
    }

    public TValue? Deserialize<TValue>(byte[] serializerData)
    {
        _logger.LogWarning("Deserialize is null, use MessagePack or SystemTextJson Serializer");
        return default;
    }

    public object? Deserialize(byte[] serializerData, Type valueType)
    {
        _logger.LogWarning("Deserialize is null, use MessagePack or SystemTextJson Serializer");
        return default;
    }
}