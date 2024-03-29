using System.Collections.Concurrent;
using BeniceSoft.Abp.Ddd.Domain;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace BeniceSoft.Abp.EntityFrameworkCore;

public class EntityFrameworkCoreEntityTableNameResolver<TContext> : IEntityTableNameResolver
    where TContext : DbContext
{
    private static readonly ConcurrentDictionary<Type, string> EntityTableCache = new();

    private readonly TContext _dbContext;

    public EntityFrameworkCoreEntityTableNameResolver(TContext dbContext)
    {
        _dbContext = dbContext;
    }

    public string GetTableName<TEntity>() where TEntity : class, IEntity
    {
        var entityType = typeof(TEntity);
        if (EntityTableCache.TryGetValue(entityType, out var tableName)) return tableName;

        tableName = _dbContext.Model.FindEntityType(entityType)?.GetTableName();
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new UserFriendlyException($"Unable to resolve the table name for entity type {entityType.GetFullNameWithAssemblyName()}");
        }

        EntityTableCache.TryAdd(entityType, tableName);
        return tableName;
    }
}