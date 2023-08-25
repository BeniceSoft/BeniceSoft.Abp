using BeniceSoft.Abp.Extensions.Caching.Abstractions.Interfaces;
using MessagePack;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.Extensions.Caching.MessagePack;

public class MessagePackCacheValueSerializer : ICacheValueSerializer, ISingletonDependency
{
    public string Name => "MessagePack";
    
    public byte[] Serialize<TValue>(TValue data)
    {
        return MessagePackSerializer.Serialize(data);
    }

    public TValue? Deserialize<TValue>(byte[] serializerData)
    {
        return MessagePackSerializer.Deserialize<TValue>(serializerData);
    }

    public object? Deserialize(byte[] serializerData, Type valueType)
    {
        return MessagePackSerializer.Deserialize(valueType, serializerData);
    }
}