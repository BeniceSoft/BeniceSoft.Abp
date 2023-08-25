using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.Ddd.Domain;

public interface IQueryableWrapperFactory : ISingletonDependency
{
    IQueryableWrapper<TEntity> CreateWrapper<TEntity>(IQueryable<TEntity> queryable) where TEntity : class;
}

public class NullQueryableWrapperFactory : IQueryableWrapperFactory
{
    public IQueryableWrapper<TEntity> CreateWrapper<TEntity>(IQueryable<TEntity> queryable) where TEntity : class
    {
        return null!;
    }
}