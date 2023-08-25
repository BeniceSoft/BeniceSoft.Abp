using System.Linq.Expressions;

namespace BeniceSoft.Abp.Extensions.DynamicQuery.EfCore.Extensions;

public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b) {    

        ParameterExpression p = a.Parameters[0];

        SubstExpressionVisitor visitor = new SubstExpressionVisitor();
        visitor.subst[b.Parameters[0]] = p;

        Expression body = Expression.AndAlso(a.Body, visitor.Visit(b.Body));
        return Expression.Lambda<Func<T, bool>>(body, p);
    }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b) {    

        ParameterExpression p = a.Parameters[0];

        SubstExpressionVisitor visitor = new SubstExpressionVisitor();
        visitor.subst[b.Parameters[0]] = p;

        Expression body = Expression.OrElse(a.Body, visitor.Visit(b.Body));
        return Expression.Lambda<Func<T, bool>>(body, p);
    }   
    
    private class SubstExpressionVisitor : ExpressionVisitor {
        public Dictionary<Expression, Expression> subst = new();

        protected override Expression VisitParameter(ParameterExpression node) {
            Expression newValue;
            if (subst.TryGetValue(node, out newValue)) {
                return newValue;
            }
            return node;
        }
    }
}