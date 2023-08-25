using System.Linq.Expressions;
using BeniceSoft.Abp.Extensions.DynamicQuery.Abstractions.Model;

namespace BeniceSoft.Abp.Ddd.Domain;

/// <summary>
/// 查询包装器
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IQueryableWrapper<TEntity> where TEntity : class
{
    /// <summary>
    /// 不追踪查询
    /// </summary>
    /// <returns></returns>
    IQueryableWrapper<TEntity> AsNoTracking();

    /// <summary>
    /// 加载导航属性
    /// </summary>
    /// <param name="propertySelectors"></param>
    /// <returns></returns>
    IQueryableWrapper<TEntity> Include(params Expression<Func<TEntity, object>>[] propertySelectors);

    /// <summary>
    /// 按照已排序集合排序
    /// </summary>
    /// <param name="keySelector"></param>
    /// <param name="sortedList"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <returns></returns>
    [Obsolete]
    IQueryableWrapper<TEntity> OrderBySortedList<TKey>(Expression<Func<TEntity, TKey>> keySelector, List<TKey> sortedList);

    /// <summary>
    /// 正序排序
    /// </summary>
    /// <param name="keySelector"></param>
    /// <returns></returns>
    IQueryableWrapper<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> keySelector);

    /// <summary>
    /// 倒序排序
    /// </summary>
    /// <param name="keySelector"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <returns></returns>
    IQueryableWrapper<TEntity> OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> keySelector);

    /// <summary>
    /// 通过关键字查询
    /// </summary>
    /// <param name="searchKey"></param>
    /// <param name="propertySelectors"></param>
    /// <returns></returns>
    IQueryableWrapper<TEntity> SearchByKey(string? searchKey, params Expression<Func<TEntity, object?>>[] propertySelectors);

    /// <summary>
    /// 条件查询
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IQueryableWrapper<TEntity> WhereIf(bool condition, Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 动态查询
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    IQueryableWrapper<TEntity> DynamicQueryBy(IDynamicQueryRequest request);

    /// <summary>
    /// 分页
    /// </summary>
    /// <param name="skipCount"></param>
    /// <param name="maxResultCount"></param>
    /// <returns></returns>
    IQueryableWrapper<TEntity> PageBy(int skipCount, int maxResultCount);

    /// <summary>
    /// 转换成 Queryable
    /// </summary>
    /// <returns></returns>
    IQueryable<TEntity> AsQueryable();

    /// <summary>
    /// 执行查询
    /// </summary>
    /// <returns></returns>
    Task<List<TEntity>> ToListAsync();

    /// <summary>
    /// 总数
    /// </summary>
    /// <returns></returns>
    Task<int> CountAsync();

    Task<TEntity?> FirstOrDefaultAsync();

    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
}