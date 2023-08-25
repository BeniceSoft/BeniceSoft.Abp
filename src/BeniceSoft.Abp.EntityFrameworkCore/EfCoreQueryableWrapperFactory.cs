using BeniceSoft.Abp.Ddd.Domain;

namespace BeniceSoft.Abp.EntityFrameworkCore;

public class EfCoreQueryableWrapperFactory : IQueryableWrapperFactory
{
    public IQueryableWrapper<TEntity> CreateWrapper<TEntity>(IQueryable<TEntity> queryable) where TEntity : class
    {
        return new EfCoreQueryableWrapper<TEntity>(queryable);
    }
}