using System.Text.Json;
using BeniceSoft.Abp.Extensions.Caching.Abstractions.Interfaces;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.Extensions.Caching.SystemTextJson;

public class SystemTextJsonCacheValueSerializer : ICacheValueSerializer, ISingletonDependency
{
    public string Name => "SystemTextJson";
    
    public byte[] Serialize<TValue>(TValue data)
    {
        return JsonSerializer.SerializeToUtf8Bytes(data);
    }

    public TValue? Deserialize<TValue>(byte[] serializerData)
    {
        return JsonSerializer.Deserialize<TValue>(serializerData);
    }

    public object? Deserialize(byte[] serializerData, Type valueType)
    {
        return JsonSerializer.Deserialize(serializerData, valueType);
    }
}