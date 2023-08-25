using System.Linq.Expressions;
using BeniceSoft.Abp.Auth.Core.Enums;

namespace BeniceSoft.Abp.Auth.Repository.Expression
{
    public class ExpreBuilder<T> where T : class
    {
        private readonly List<PropertySettings> _configs = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public ExpreBuilder<T> Property(string propertyName, Expression<Func<T, bool>> expression)
        {
            Configure(new PropertySettings(propertyName, expression));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public ExpreBuilder<T> Property(Expression<Func<T, object>> columnName, Expression<Func<T, bool>> expression) => Property(columnName.GetPropertyName(), expression);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="operator"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public ExpreBuilder<T> Property(string propertyName, ExprOperator @operator, string fieldName = "")
        {
            Configure(new PropertySettings(propertyName, @operator, fieldName));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="operator"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public ExpreBuilder<T> Property(Expression<Func<T, object>> columnName, ExprOperator @operator, string fieldName = "") => Property(columnName.GetPropertyName(), @operator, fieldName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public ExpreBuilder<T> Ignore(Expression<Func<T, object>> columnName) => Ignore(columnName.GetPropertyName());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public ExpreBuilder<T> Ignore(string propertyName)
        {
            Configure(new PropertySettings(propertyName));
            return this;
        }

        private void Configure(PropertySettings setting)
        {
            if (_configs.Any(t => t.PropertyName == setting.PropertyName))
            {
                throw new InvalidOperationException($"PropertyName:{setting.PropertyName} already configured");
            }

            _configs.Add(setting);
        }

        private class PropertySettings
        {
            internal PropertySettings(string propertyName, Expression<Func<T, bool>> expression)
            {
                PropertyName = propertyName;
                Expression = expression;
            }

            internal PropertySettings(string propertyName, ExprOperator @operator, string fieldName = "")
            {
                PropertyName = propertyName;
                Operator = @operator;
                FieldName = fieldName;
            }

            internal PropertySettings(string propertyName)
            {
                PropertyName = propertyName;
                Ignore = true;
            }

            internal string PropertyName { get; }

            internal string FieldName { get; set; }

            internal Expression<Func<T, bool>> Expression { get; }

            internal ExprOperator Operator { get; } = ExprOperator.Equal;

            internal bool Ignore { get; } = false;
        }
    }

    
}
