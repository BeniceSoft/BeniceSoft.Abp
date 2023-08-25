using System.Linq.Expressions;
using System.Reflection;
using BeniceSoft.Abp.Ddd.Domain;
using BeniceSoft.Abp.Extensions.DynamicQuery.Abstractions.Model;
using BeniceSoft.Abp.Extensions.DynamicQuery.EfCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Volo.Abp;

namespace BeniceSoft.Abp.EntityFrameworkCore;

public class EfCoreQueryableWrapper<TEntity> : IQueryableWrapper<TEntity> where TEntity : class
{
    private IQueryable<TEntity> _queryable;

    public EfCoreQueryableWrapper(IQueryable<TEntity> queryable)
    {
        _queryable = queryable;
    }

    public IQueryableWrapper<TEntity> AsNoTracking()
    {
        _queryable = _queryable.AsNoTracking();
        return this;
    }

    public IQueryableWrapper<TEntity> Include(params Expression<Func<TEntity, object>>[] propertySelectors)
    {
        if (!propertySelectors.IsNullOrEmpty())
        {
            foreach (var propertySelector in propertySelectors)
            {
                _queryable = _queryable.Include(propertySelector);
            }
        }

        return this;
    }

    public IQueryableWrapper<TEntity> OrderBySortedList<TKey>(Expression<Func<TEntity, TKey>> keySelector, List<TKey> sortedList)
    {
        // 无法翻译成 SQL
        // if (sortedList.Any())
        // {
        //     var memberInfo = keySelector.GetMemberAccess();
        //     var memberExpression = Expression.MakeMemberAccess(_entityParameterExp, memberInfo);
        //
        //     var indexOfMethodInfo = typeof(List<>).MakeGenericType(typeof(TKey))
        //         .GetMethods()
        //         .Single(x => x.Name == "IndexOf" && x.GetParameters().Length == 1 && x.GetParameters().First().ParameterType == typeof(TKey));
        //     var listParameterExp = Expression.Constant(sortedList);
        //
        //     var callExp = Expression.Call(listParameterExp, indexOfMethodInfo, memberExpression);
        //
        //     var lambda = Expression.Lambda<Func<TEntity, int>>(callExp, _entityParameterExp);
        //
        //     _queryable = _queryable.OrderBy(lambda);
        // }

        throw new NotImplementedException();
    }

    public IQueryableWrapper<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> keySelector)
    {
        _queryable = _queryable.OrderBy(keySelector);
        return this;
    }

    public IQueryableWrapper<TEntity> OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> keySelector)
    {
        _queryable = _queryable.OrderByDescending(keySelector);
        return this;
    }

    public IQueryableWrapper<TEntity> SearchByKey(string searchKey, params Expression<Func<TEntity, object>>[] propertySelectors)
    {
        if (!searchKey.IsNullOrWhiteSpace() && propertySelectors.Any())
        {
            var searchExp = Expression.Constant(searchKey);

            // 最终查询的条件
            var predicateExp = Expression.Lambda<Func<TEntity, bool>>(_falseConstantExp, _entityParameterExp);

            foreach (LambdaExpression propertySelector in propertySelectors)
            {
                var memberInfo = propertySelector.GetMemberAccess();
                if (memberInfo.MemberType != MemberTypes.Property)
                {
                    throw new AbpException($"{propertySelector}不是有效的属性");
                }

                var propertyInfo = (memberInfo as PropertyInfo)!;
                Expression propertyExp = Expression.Property(_entityParameterExp, propertyInfo);
                var nullCheckExp = BuildNullCheckExpression(propertyExp);

                if (propertyInfo.PropertyType != typeof(string))
                {
                    var toStringMethodInfo = propertyInfo.PropertyType
                        .GetMethod(nameof(Object.ToString), Type.EmptyTypes)!;
                    propertyExp = Expression.Call(propertyExp, toStringMethodInfo);
                }

                var callExp = Expression.Call(propertyExp, ContainsMethodInfo, searchExp);
                var predicate = Expression.AndAlso(nullCheckExp, callExp);
                var lambdaExp = Expression.Lambda<Func<TEntity, bool>>(predicate, _entityParameterExp);

                predicateExp = PredicateBuilder.Or(predicateExp, lambdaExp);
            }

            _queryable = _queryable.Where(predicateExp);
        }

        return this;
    }

    public IQueryableWrapper<TEntity> WhereIf(bool condition, Expression<Func<TEntity, bool>> predicate)
    {
        if (condition)
        {
            _queryable = _queryable.Where(predicate);
        }

        return this;
    }

    public IQueryableWrapper<TEntity> DynamicQueryBy(IDynamicQueryRequest request)
    {
        _queryable = _queryable.DynamicQueryBy(request, skipNullableCheck: true);

        return this;
    }

    public IQueryableWrapper<TEntity> PageBy(int skipCount, int maxResultCount)
    {
        if (skipCount < 0)
        {
            throw new ArgumentException("skipCount cannot be less then 0");
        }

        _queryable = _queryable.Skip(skipCount).Take(maxResultCount);

        return this;
    }

    public IQueryable<TEntity> AsQueryable() => _queryable;

    public Task<List<TEntity>> ToListAsync() => _queryable.ToListAsync();

    public Task<int> CountAsync() => _queryable.CountAsync();

    public Task<TEntity?> FirstOrDefaultAsync() => _queryable.FirstOrDefaultAsync();

    public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate) => _queryable.FirstOrDefaultAsync(predicate);

    #region Private Methods

    private static Expression BuildNullCheckExpression(Expression propertyExp)
    {
        var isNullable = !propertyExp.Type.IsValueType ||
                         Nullable.GetUnderlyingType(propertyExp.Type) is not null;

        if (isNullable)
        {
            return Expression.NotEqual(
                propertyExp,
                Expression.Constant(propertyExp.Type.GetDefaultValue(),
                    propertyExp.Type));
        }

        return Expression.Constant(true, typeof(bool));
    }

    // ReSharper disable once StaticMemberInGenericType
    private static readonly MethodInfo ContainsMethodInfo = typeof(string)
        .GetMethod(nameof(string.Contains), new[] { typeof(string) })!;

    private readonly ConstantExpression _falseConstantExp = Expression.Constant(false);
    private readonly ParameterExpression _entityParameterExp = Expression.Parameter(typeof(TEntity), "x");

    #endregion Private Methods
}
