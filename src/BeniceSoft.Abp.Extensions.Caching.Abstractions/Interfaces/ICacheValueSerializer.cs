namespace BeniceSoft.Abp.Extensions.Caching.Abstractions.Interfaces;

public interface ICacheValueSerializer
{
    string Name { get; }
    
    byte[] Serialize<TValue>(TValue data);

    TValue? Deserialize<TValue>(byte[] serializerData);
    
    object? Deserialize(byte[] serializerData, Type valueType);
}