using System.Linq.Expressions;
using BeniceSoft.Abp.Auth.Core;
using BeniceSoft.Abp.Auth.Core.Entity;
using BeniceSoft.Abp.Auth.Core.Enums;
using BeniceSoft.Abp.Auth.Core.Models;
using BeniceSoft.Abp.Auth.Repository.Expression;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace BeniceSoft.Abp.Auth.Repository;

public static class RepositoryExtensions
{
    public static async Task<IQueryable<TEntity>> GetAuthQueryableAsync<TEntity>(this IRepository<TEntity> repository,
        IUserPermission? userPermission, string tableName)
        where TEntity : class, IEntity
    {
        var queryable = await repository.GetQueryableAsync();
        var data = userPermission?
            .RowPermissions?
            .Where(c => c.TableName == tableName)
            .ToList();
        if (!(data?.Any() ?? false))
        {
            return queryable;
        }

        var exp = GenerateExp<TEntity>(data, tableName, userPermission?.UserId);
        return queryable.Where(exp);
    }

    public static async Task<IQueryable<TEntity>> GetAuthQueryableAsync<TEntity>(this IQueryable<TEntity> queryable,
        IUserPermission? userPermission, string tableName)
        where TEntity : class, IEntity
    {
        var data = userPermission?.RowPermissions?.Where(c => c.TableName == tableName).ToList();
        if (!(data?.Any() ?? false))
        {
            return queryable;
        }

        await Task.Delay(1);
        var exp = GenerateExp<TEntity>(data, tableName, userPermission?.UserId);
        return queryable.Where(exp);
    }

    private static Expression<Func<TEntity, bool>> GenerateExp<TEntity>(List<RowPermission> data, string tableName, Guid? userId)
        where TEntity : class, IEntity
    {
        // 添加业务单据的 与我有关的权限
        var groupExpList = new List<Expression<Func<TEntity, bool>>>();
        if (typeof(IHaveOwnerId).IsAssignableFrom(typeof(TEntity)))
        {
            if (userId.HasValue)
            {
                var ownerExp = ExprBuilder.Create<TEntity>(nameof(IHaveOwnerId.OwnerId), userId, ExprOperator.Equal);
                groupExpList.Add(ownerExp);
            }
        }

        foreach (var dataPermission in data)
        {
            foreach (var group in dataPermission.ConditionGroups)
            {
                var currentGroupExp = ExprBuilder.True<TEntity>();
                foreach (var condition in group.Conditions)
                {
                    if (condition.IsDataSuperAdmin) //当前字段超管权限，默认查询字段的所有数据
                    {
                        continue;
                    }

                    // 不是超管，又没有勾选具体的值，返回false,一条数据都查不到
                    if (condition.Values.Count < 1)
                    {
                        currentGroupExp = currentGroupExp.AndEx(ExprBuilder.False<TEntity>());
                    }
                    else
                    {
                        // 组内条件表达式
                        _ = int.TryParse(condition.Operator, out int result);
                        currentGroupExp = currentGroupExp.AndEx(condition.ColumnName, condition.Values, (ExprOperator)result);
                    }
                }

                groupExpList.Add(currentGroupExp);
            }
        }

        var filterExpression = groupExpList.FirstOrDefault();
        foreach (var item in groupExpList)
        {
            if (item == filterExpression)
            {
                continue;
            }

            filterExpression = filterExpression?.OrEx(item);
        }

        if (filterExpression == null)
        {
            filterExpression = ExprBuilder.False<TEntity>();
        }

        return filterExpression;
    }
}