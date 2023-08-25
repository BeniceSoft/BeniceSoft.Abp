using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using BeniceSoft.Abp.Extensions.DynamicQuery.Abstractions;
using BeniceSoft.Abp.Extensions.DynamicQuery.Abstractions.Model;
using Operators = BeniceSoft.Abp.Extensions.DynamicQuery.Abstractions.DynamicQueryConstants.Operators;
using Relations = BeniceSoft.Abp.Extensions.DynamicQuery.Abstractions.DynamicQueryConstants.Relations;
using TypeNames = BeniceSoft.Abp.Extensions.DynamicQuery.Abstractions.DynamicQueryConstants.TypeNames;

namespace BeniceSoft.Abp.Extensions.DynamicQuery.EfCore.Extensions;

public static class DynamicQueryExtensions
{
    public static IQueryable<T> DynamicQueryBy<T>(this IEnumerable<T> queryable, IDynamicQueryRequest request)
    {
        return DynamicQueryBy(queryable.AsQueryable(), request);
    }

    public static IQueryable<T> DynamicQueryBy<T>(this IList<T> queryable, IDynamicQueryRequest request)
    {
        return DynamicQueryBy(queryable.AsQueryable(), request);
    }

    /// <summary>
    /// 动态查询
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="request">查询请求</param>
    /// <param name="skipNullableCheck">是否跳过空值检查</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IQueryable<T> DynamicQueryBy<T>(this IQueryable<T> queryable, IDynamicQueryRequest request, bool skipNullableCheck = false)
    {
        return DynamicQueryBy(queryable, request, out _, skipNullableCheck);
    }

    /// <summary>
    /// 动态查询
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="request">查询请求</param>
    /// <param name="queryString">生成的表达式</param>
    /// <param name="skipNullableCheck">是否跳过空值检查</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IQueryable<T> DynamicQueryBy<T>(this IQueryable<T> queryable, IDynamicQueryRequest request, out string queryString,
        bool skipNullableCheck = false)
    {
        var expression = BuildLambdaExpression<T>(request, out queryString, skipNullableCheck);

        if (expression is null) return queryable;

        var whereCallExp = Expression.Call(
            typeof(Queryable),
            nameof(Queryable.Where),
            new[] { queryable.ElementType },
            queryable.Expression,
            expression);

        var filteredQueryable = queryable.Provider.CreateQuery<T>(whereCallExp);

        return filteredQueryable;
    }

    private static Expression<Func<T, bool>>? BuildLambdaExpression<T>(IDynamicQueryRequest request, out string queryString, bool skipNullableCheck)
    {
        queryString = string.Empty;
        if (!(request.ConditionGroups?.Any() ?? false)) return null;

        var trueConstantExp = Expression.Constant(true);
        // var properties = typeof(T).GetProperties();
        var parameterExp = Expression.Parameter(typeof(T), "x");

        // 最终查询的条件
        var predicateExp = Expression.Lambda<Func<T, bool>>(trueConstantExp, parameterExp);

        for (var i = 0; i < request.ConditionGroups.Count; i++)
        {
            var conditionGroup = request.ConditionGroups[i];

            // 条件组组合后的条件
            var conditionGroupExp = Expression.Lambda<Func<T, bool>>(trueConstantExp, parameterExp);

            for (var j = 0; j < conditionGroup.Conditions.Count; j++)
            {
                var condition = conditionGroup.Conditions[j];

                if (string.IsNullOrWhiteSpace(condition.FieldName)) continue;

                var type = GetCSharpType(condition.FieldType);
                Expression conditionExp;

                var propertyList = condition.FieldName.Split('.');
                if (propertyList.Length > 1)
                {
                    using var propertiesEnumerator = propertyList.AsEnumerable().GetEnumerator();
                    conditionExp = BuildNestedExpression(parameterExp, propertiesEnumerator, condition, type, skipNullableCheck);
                }
                else
                {
                    var propertyExp = Expression.Property(parameterExp, condition.FieldName);
                    // Date 类型
                    if (condition.FieldType == TypeNames.Date)
                    {
                        propertyExp = Expression.MakeMemberAccess(propertyExp, typeof(DateTime).GetProperty(nameof(DateTime.Date))!);
                    }

                    conditionExp = BuildOperatorExpression(propertyExp, condition, type, skipNullableCheck);
                }

                var lambdaExp = Expression.Lambda<Func<T, bool>>(conditionExp, parameterExp);

                if (j == 0) condition.Relation = Relations.And;

                // 每个条件组中多个条件组合后
                conditionGroupExp = condition.Relation switch
                {
                    Relations.And => conditionGroupExp.And(lambdaExp),
                    Relations.Or => conditionGroupExp.Or(lambdaExp),
                    _ => throw new DynamicQueryException("Relation invalid")
                };
            }

            if (i == 0) conditionGroup.Relation = Relations.And;

            predicateExp = conditionGroup.Relation switch
            {
                Relations.And => predicateExp.And(conditionGroupExp),
                Relations.Or => predicateExp.Or(conditionGroupExp),
                _ => throw new DynamicQueryException("Relation invalid")
            };
        }

        queryString = predicateExp.ToString();
        return predicateExp;
    }

    /// <summary>
    /// 构建嵌套表达式
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="propertiesEnumerator"></param>
    /// <param name="condition"></param>
    /// <param name="type"></param>
    /// <param name="skipNullableCheck"></param>
    /// <returns></returns>
    private static Expression BuildNestedExpression(Expression expression, IEnumerator<string> propertiesEnumerator,
        DynamicQueryCondition condition, Type type, bool skipNullableCheck)
    {
        while (propertiesEnumerator.MoveNext())
        {
            var propertyName = propertiesEnumerator.Current;
            var property = expression.Type.GetProperty(propertyName)!;
            expression = Expression.Property(expression, property);

            var propertyType = property.PropertyType;
            // property is IEnumerable<TItem>
            if (propertyType != typeof(string) && propertyType.IsGenericList())
            {
                // TItem
                var elementType = propertyType.GetGenericArguments()[0];
                var predicateFuncType = typeof(Func<,>).MakeGenericType(elementType, typeof(bool));
                var parameterExp = Expression.Parameter(elementType);

                var body = BuildNestedExpression(parameterExp, propertiesEnumerator, condition, type, skipNullableCheck);
                var predicate = Expression.Lambda(predicateFuncType, body, parameterExp);

                var queryable = Expression.Call(typeof(Queryable), nameof(Queryable.AsQueryable), new[] { elementType }, expression);

                return Expression.Call(
                    typeof(Queryable),
                    nameof(Queryable.Any),
                    new[] { elementType },
                    queryable,
                    predicate);
            }
        }

        return BuildOperatorExpression(expression, condition, type, skipNullableCheck);
    }

    /// <summary>
    /// 构建比较表达式
    /// </summary>
    /// <param name="propertyExp"></param>
    /// <param name="condition"></param>
    /// <param name="type"></param>
    /// <param name="skipNullableCheck"></param>
    /// <returns></returns>
    /// <DynamicQueryException cref="DynamicQueryException"></DynamicQueryException>
    private static Expression BuildOperatorExpression(Expression propertyExp, DynamicQueryCondition condition, Type type, bool skipNullableCheck)
    {
        var @operator = condition.Operator.ToLower();
        var expression = @operator switch
        {
            Operators.Equal => Equal(type, condition.Value, propertyExp, skipNullableCheck),
            Operators.NotEqual => NotEqual(type, condition.Value, propertyExp, skipNullableCheck),
            Operators.GreaterThan => GreaterThan(type, condition.Value, propertyExp, skipNullableCheck),
            Operators.GreaterThanOrEqual => GreaterThanOrEqual(type, condition.Value, propertyExp, skipNullableCheck),
            Operators.LessThan => LessThan(type, condition.Value, propertyExp, skipNullableCheck),
            Operators.LessThanOrEqual => LessThanOrEqual(type, condition.Value, propertyExp, skipNullableCheck),
            Operators.StartsWith => StartsWith(type, condition.Value, propertyExp, skipNullableCheck),
            Operators.EndsWith => EndsWith(type, condition.Value, propertyExp, skipNullableCheck),
            Operators.Contains => Contains(type, condition.Value, propertyExp, skipNullableCheck),
            Operators.NotContains => NotContains(type, condition.Value, propertyExp, skipNullableCheck),
            Operators.Between => Between(type, condition.Value, propertyExp, skipNullableCheck),
            Operators.In => In(type, condition.Value, propertyExp, skipNullableCheck),
            Operators.NotIn => NotIn(type, condition.Value, propertyExp, skipNullableCheck),
            _ => throw new DynamicQueryException($"Unknown expression operator: {condition.Operator}")
        };

        return expression;
    }

    #region Operator Expressions

    private static Expression Between(Type type, List<string> value, Expression propertyExp, bool skipNullableCheck)
    {
        var values = GetConstantExpressions(type, value);
        if (values.Count < 2) throw new DynamicQueryException("Between needs two values");

        var belowExp = Expression.GreaterThanOrEqual(propertyExp, Expression.Convert(values[0], propertyExp.Type));
        var aboveExp = Expression.LessThanOrEqual(propertyExp, Expression.Convert(values[1], propertyExp.Type));

        return Expression.And(belowExp, aboveExp);
    }

    private static Expression NotContains(Type type, List<string> value, Expression propertyExp, bool skipNullableCheck)
        => Expression.Not(Contains(type, value, propertyExp, skipNullableCheck));

    private static Expression Contains(Type type, List<string> value, Expression propertyExp, bool skipNullableCheck)
    {
        var firstValue = EnsureFirstValueNotEmpty(value);
        var nullCheckExp = skipNullableCheck ? Expression.Constant(true) : GetNullCheckExpression(propertyExp);

        // 如果是集合 x => x.RoleIds.Contains()
        if (propertyExp.Type.IsGenericList())
        {
            var genericType = propertyExp.Type.GetGenericArguments()[0];

            var containersMethod = ContainsMethodInfo
                .MakeGenericMethod(genericType);

            var typeConverter = TypeDescriptor.GetConverter(genericType);
            var constantExp = Expression.Constant(typeConverter.ConvertFromString(firstValue), genericType);
            var containsExp = Expression.Call(
                null,
                containersMethod,
                propertyExp,
                constantExp);

            return Expression.AndAlso(nullCheckExp, containsExp);
        }

        var valueExp = Expression.Constant(firstValue.ToLower(), typeof(string));
        Expression? toStringExp = null;
        if (propertyExp.Type.IsGuid())
        {
            toStringExp = Expression.Call(
                propertyExp,
                typeof(Guid).GetMethod(nameof(Guid.ToString), Type.EmptyTypes)!);
        }

        var containsMethod = (toStringExp ?? propertyExp).Type.GetMethod("Contains", new[] { typeof(string) });
        if (containsMethod is null)
            throw new DynamicQueryException($"Type {propertyExp.Type} not defined Contains.");

        var toLowerExp = Expression.Call(
            toStringExp ?? propertyExp,
            typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!);

        return Expression.AndAlso(nullCheckExp, Expression.Call(toLowerExp, containsMethod, valueExp));
    }

    private static Expression EndsWith(Type type, List<string> value, Expression propertyExp, bool skipNullableCheck)
    {
        var firstValue = EnsureFirstValueNotEmpty(value);
        var valueExp = Expression.Constant(firstValue.ToLower(), typeof(string));
        var nullCheckExp = skipNullableCheck ? Expression.Constant(true) : GetNullCheckExpression(propertyExp);

        Expression? toStringExp = null;
        if (propertyExp.Type.IsGuid())
        {
            toStringExp = Expression.Call(
                propertyExp,
                typeof(Guid).GetMethod(nameof(Guid.ToString), Type.EmptyTypes)!);
        }

        var endsWithMethod = (toStringExp ?? propertyExp).Type.GetMethod("EndsWith", new[] { typeof(string) });
        if (endsWithMethod is null)
            throw new DynamicQueryException($"Type {propertyExp.Type} not defined EndsWith.");

        var toLowerExp = Expression.Call(
            toStringExp ?? propertyExp,
            typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!);

        return Expression.AndAlso(nullCheckExp, Expression.Call(toLowerExp, endsWithMethod, valueExp));
    }

    private static Expression StartsWith(Type type, List<string> value, Expression propertyExp, bool skipNullableCheck)
    {
        var firstValue = EnsureFirstValueNotEmpty(value);
        var valueExp = Expression.Constant(firstValue.ToLower(), typeof(string));
        var nullCheckExp = skipNullableCheck ? Expression.Constant(true) : GetNullCheckExpression(propertyExp);

        Expression? toStringExp = null;
        if (propertyExp.Type.IsGuid())
        {
            toStringExp = Expression.Call(
                propertyExp,
                typeof(Guid).GetMethod(nameof(Guid.ToString), Type.EmptyTypes)!);
        }

        var startsWithMethod = (toStringExp ?? propertyExp).Type.GetMethod("StartsWith", new[] { typeof(string) });
        if (startsWithMethod is null)
            throw new DynamicQueryException($"Type {propertyExp.Type} not defined StartsWith.");

        var toLowerExp = Expression.Call(
            toStringExp ?? propertyExp,
            typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!);

        return Expression.AndAlso(nullCheckExp, Expression.Call(toLowerExp, startsWithMethod, valueExp));
    }

    private static Expression LessThanOrEqual(Type type, List<string> value, Expression propertyExp, bool skipNullableCheck)
    {
        var valueExp = GetConstantExpressions(type, value).First();

        return Expression.LessThanOrEqual(propertyExp, Expression.Convert(valueExp, propertyExp.Type));
    }

    private static Expression LessThan(Type type, List<string> value, Expression propertyExp, bool skipNullableCheck)
    {
        var valueExp = GetConstantExpressions(type, value).First();

        return Expression.LessThan(propertyExp, Expression.Convert(valueExp, propertyExp.Type));
    }

    private static Expression GreaterThanOrEqual(Type type, List<string> value, Expression propertyExp, bool skipNullableCheck)
    {
        var valueExp = GetConstantExpressions(type, value).First();

        return Expression.GreaterThanOrEqual(propertyExp, Expression.Convert(valueExp, propertyExp.Type));
    }

    private static Expression GreaterThan(Type type, List<string> value, Expression propertyExp, bool skipNullableCheck)
    {
        var valueExp = GetConstantExpressions(type, value).First();

        return Expression.GreaterThan(propertyExp, Expression.Convert(valueExp, propertyExp.Type));
    }

    private static Expression NotEqual(Type type, List<string> value, Expression propertyExp, bool skipNullableCheck)
        => Expression.Not(Equal(type, value, propertyExp, skipNullableCheck));

    private static Expression Equal(Type type, List<string> value, Expression propertyExp, bool skipNullableCheck)
    {
        var valueExp = GetConstantExpressions(type, value).First();

        if (type == typeof(string))
        {
            var nullCheckExp = skipNullableCheck ? Expression.Constant(true) : GetNullCheckExpression(propertyExp);

            Expression? toStringExp = null;
            if (propertyExp.Type.IsGuid())
            {
                toStringExp = Expression.Call(
                    propertyExp,
                    typeof(Guid).GetMethod(nameof(Guid.ToString), Type.EmptyTypes)!);
            }

            var toLowerExp = Expression.Call(
                toStringExp ?? propertyExp,
                typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!);
            var valueToLowerExp = Expression.Call(
                valueExp,
                typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!);
            return Expression.AndAlso(nullCheckExp, Expression.Equal(toLowerExp, valueToLowerExp));
        }

        return Expression.Equal(propertyExp, Expression.Convert(valueExp, propertyExp.Type));
    }

    private static Expression In(Type type, List<string> value, Expression propertyExp, bool skipNullableCheck)
    {
        var values = GetConstantExpressions(type, value);
        var nullCheck = skipNullableCheck ? Expression.Constant(true) : GetNullCheckExpression(propertyExp);

        Expression containsExp;
        if (propertyExp.Type.IsGenericList())
        {
            var genericType = propertyExp.Type.GetGenericArguments()[0];
            var containsMethod = ContainsMethodInfo.MakeGenericMethod(genericType);

            containsExp = Expression.Call(
                null,
                containsMethod,
                propertyExp,
                Expression.Convert(values[0], genericType));

            if (values.Count > 1)
            {
                var index = 1;
                while (index < values.Count)
                {
                    containsExp = Expression.Or(containsExp,
                        Expression.Call(null, containsMethod, propertyExp, Expression.Convert(values[index], genericType)));
                    index++;
                }
            }

            return Expression.AndAlso(nullCheck, containsExp);
        }

        if (values.Count > 1)
        {
            if (type == typeof(string))
            {
                Expression? toStringExp = null;
                if (propertyExp.Type.IsGuid())
                {
                    toStringExp = Expression.Call(
                        propertyExp,
                        propertyExp.Type.GetMethod(nameof(Guid.ToString), Type.EmptyTypes)!);
                }

                var toLowerExp = Expression.Call(
                    toStringExp ?? propertyExp,
                    typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!);

                var valueToLowerExp = Expression.Call(
                    values[0],
                    typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!);
                containsExp = Expression.Equal(toLowerExp, valueToLowerExp);

                var index = 1;
                while (index < values.Count)
                {
                    valueToLowerExp = Expression.Call(
                        values[index],
                        typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!);
                    containsExp = Expression.Or(
                        containsExp,
                        Expression.Equal(
                            Expression.Call(toStringExp ?? propertyExp, typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!),
                            valueToLowerExp));
                    index++;
                }
            }
            else
            {
                containsExp = Expression.Equal(propertyExp, Expression.Convert(values[0], propertyExp.Type));
                var index = 1;
                while (index < values.Count)
                {
                    containsExp = Expression.Or(
                        containsExp,
                        Expression.Equal(
                            propertyExp, Expression.Convert(values[index], propertyExp.Type)));
                    index++;
                }
            }
        }
        else
        {
            if (type == typeof(string))
            {
                Expression? toStringExp = null;
                if (propertyExp.Type.IsGuid())
                {
                    toStringExp = Expression.Call(
                        propertyExp,
                        propertyExp.Type.GetMethod(nameof(Guid.ToString), Type.EmptyTypes)!);
                }

                var toLowerExp = Expression.Call(
                    toStringExp ?? propertyExp,
                    typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!);
                var valueToLower = Expression.Call(
                    values[0],
                    typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!);
                containsExp = Expression.Equal(toLowerExp, valueToLower);
            }
            else
            {
                containsExp = Expression.Equal(propertyExp, Expression.Convert(values[0], propertyExp.Type));
            }
        }

        return Expression.And(nullCheck, containsExp);
    }

    private static Expression NotIn(Type type, List<string> value, Expression propertyExp, bool skipNullableCheck)
        => Expression.Not(In(type, value, propertyExp, skipNullableCheck));

    #endregion

    private static string EnsureFirstValueNotEmpty(List<string> value)
    {
        var firstValue = value.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(firstValue))
        {
            throw new DynamicQueryException("Value can not be null or empty");
        }

        return firstValue;
    }

    private static List<ConstantExpression> GetConstantExpressions(Type type, List<string> value)
    {
        if (type == typeof(DateTime))
        {
            DateTime dateTime;
            var constants = new List<ConstantExpression>();
            foreach (var item in value)
            {
                var dt = DateTime.TryParse(item, out dateTime) ? (DateTime?)dateTime : null;
                constants.Add(Expression.Constant(dt, type));
            }

            return constants;
        }

        var typeConverter = TypeDescriptor.GetConverter(type);

        if (type == typeof(string))
        {
            return value
                .Select(x => Expression.Constant(x, type))
                .ToList();
        }

        return value
            .Select(x => Expression.Constant(typeConverter.ConvertFromString(x), type))
            .ToList();
    }

    private static Expression GetNullCheckExpression(Expression propertyExp)
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

    private static Type GetCSharpType(string typeName)
    {
        return typeName switch
        {
            TypeNames.Integer => typeof(int),
            TypeNames.Double => typeof(double),
            TypeNames.String => typeof(string),
            TypeNames.Date => typeof(DateTime),
            TypeNames.DateTime => typeof(DateTime),
            TypeNames.Boolean => typeof(bool),
            TypeNames.Guid => typeof(Guid),
            _ => throw new DynamicQueryException($"Unexpected data type {typeName}")
        };
    }


    private static readonly MethodInfo ContainsMethodInfo = typeof(Enumerable).GetMethods()
        .Where(x => x.Name == nameof(Enumerable.Contains))
        .Single(x => x.GetParameters().Length == 2);
}