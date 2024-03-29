using System.Collections.Concurrent;
using BeniceSoft.Abp.Ddd.Domain;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace BeniceSoft.Abp.Dapper;

public class DapperEntityTableNameResolver : IEntityTableNameResolver
{
    private static readonly ConcurrentDictionary<Type, string> EntityTableCache = new();

    public string GetTableName<TEntity>() where TEntity : class, IEntity
    {
        var entityType = typeof(TEntity);
        if (EntityTableCache.TryGetValue(entityType, out var tableName)) return tableName;

        // 直接使用实体的类名，后续可以从其他方式获取，例如读取 attribute 等等
        tableName = entityType.Name;
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new UserFriendlyException($"Unable to resolve the table name for entity type {entityType.GetFullNameWithAssemblyName()}");
        }

        EntityTableCache.TryAdd(entityType, tableName);
        return tableName;
    }
}