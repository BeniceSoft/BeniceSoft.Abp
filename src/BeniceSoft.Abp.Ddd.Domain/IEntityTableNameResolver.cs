using Volo.Abp.Domain.Entities;

namespace BeniceSoft.Abp.Ddd.Domain;

public interface IEntityTableNameResolver
{
    string GetTableName<TEntity>() where TEntity : class, IEntity;
}