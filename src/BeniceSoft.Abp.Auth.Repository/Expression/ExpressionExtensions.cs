using System.Linq.Expressions;
using BeniceSoft.Abp.Auth.Core.Enums;

namespace BeniceSoft.Abp.Auth.Repository.Expression;

public static class ExpressionExtensions
{
    /// <summary>
    /// or superposition of lambda expressions 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="aim"></param>
    /// <param name="expr"></param>
    /// <returns></returns>
    public static Expression<Func<T, bool>> OrEx<T>(this Expression<Func<T, bool>> aim, Expression<Func<T, bool>> expr)
    {
        if (aim == null || expr == null)
        {
            return aim;
        }

        var invokedExpr = System.Linq.Expressions.Expression.Invoke(expr, aim.Parameters);
        return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(System.Linq.Expressions.Expression.OrElse(aim.Body, invokedExpr), aim.Parameters);
    }

    /// <summary>
    /// or superposition of lambda expressions 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="aim"></param>
    /// <param name="propertyName"></param>
    /// <param name="propertyValue"></param>
    /// <param name="eop"></param>
    /// <returns></returns>
    public static Expression<Func<T, bool>> OrEx<T>(this Expression<Func<T, bool>> aim, string propertyName, object propertyValue, ExprOperator eop = ExprOperator.Equal)
        where T : class
    {
        var expr = ExprBuilder.Create<T>(propertyName, propertyValue, eop);
        return aim.OrEx(expr);
    }

    /// <summary>
    /// Or
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="aim"></param>
    /// <param name="columnName"></param>
    /// <param name="propertyValue"></param>
    /// <param name="eop"></param>
    /// <returns></returns>
    public static Expression<Func<T, bool>> OrEx<T>(this Expression<Func<T, bool>> aim, Expression<Func<T, object>> columnName, object propertyValue, ExprOperator eop = ExprOperator.Equal)
        where T : class => aim.OrEx(columnName.GetPropertyName(), propertyValue, eop);

    /// <summary>
    /// and superposition of lambda expressions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="aim"></param>
    /// <param name="expr"></param>
    /// <returns></returns>
    public static Expression<Func<T, bool>> AndEx<T>(this Expression<Func<T, bool>> aim, Expression<Func<T, bool>> expr)
    {
        if (aim == null || expr == null)
        {
            return aim;
        }

        var invokedExpr = System.Linq.Expressions.Expression.Invoke(expr, aim.Parameters);
        return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(System.Linq.Expressions.Expression.AndAlso(aim.Body, invokedExpr), aim.Parameters);
    }

    /// <summary>
    /// and superposition of lambda expressions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="aim"></param>
    /// <param name="propertyName"></param>
    /// <param name="propertyValue"></param>
    /// <param name="eop"></param>
    /// <returns></returns>
    public static Expression<Func<T, bool>> AndEx<T>(this Expression<Func<T, bool>> aim, string propertyName, object propertyValue, ExprOperator eop = ExprOperator.Equal)
        where T : class
    {
        var expr = ExprBuilder.Create<T>(propertyName, propertyValue, eop);
        var result = aim.AndEx(expr);
        return result;
    }

    /// <summary>
    /// And 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="aim"></param>
    /// <param name="columnName"></param>
    /// <param name="propertyValue"></param>
    /// <param name="eop"></param>
    /// <returns></returns>
    public static Expression<Func<T, bool>> AndEx<T>(this Expression<Func<T, bool>> aim, Expression<Func<T, object>> columnName, object propertyValue, ExprOperator eop = ExprOperator.Equal)
        where T : class => aim.AndEx(columnName.GetPropertyName(), propertyValue, eop);

    /// <summary>
    /// Get Property Name
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static string GetPropertyName(this System.Linq.Expressions.Expression method)
    {
        var lambda = method as LambdaExpression;

        MemberExpression memberExpr = null;

        if (lambda.Body.NodeType == ExpressionType.Convert)
        {
            memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
        }
        else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
        {
            memberExpr = lambda.Body as MemberExpression;
        }

        return memberExpr.Member.Name;
    }
}
