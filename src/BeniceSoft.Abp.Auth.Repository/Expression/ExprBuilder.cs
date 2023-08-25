using System.Linq.Expressions;
using BeniceSoft.Abp.Auth.Core.Enums;

namespace BeniceSoft.Abp.Auth.Repository.Expression
{
    public static class ExprBuilder
    {
        /// <summary>
        /// 返回一个默认为true的表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> True<T>() where T : class => t => true;

        /// <summary>
        /// 返回一个默认为false的表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> False<T>() where T : class => t => false;

        /// <summary>
        /// 创建表达式树
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnName"></param>
        /// <param name="propertyValue"></param>
        /// <param name="eop"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, object>> columnName, object propertyValue, ExprOperator eop)
            where T : class => Create<T>(columnName.GetPropertyName(), propertyValue, eop);

        /// <summary>
        /// 创建表达式树
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="propertyName">字段名</param>
        /// <param name="propertyValue">条件值(目前不支持值类型)</param>
        /// <param name="eop">操作符</param>
        /// <returns></returns>
        /// <exception cref="MissingMethodException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public static Expression<Func<T, bool>> Create<T>(string propertyName, object propertyValue, ExprOperator eop)
        where T : class
        {
            if (eop == ExprOperator.None || string.IsNullOrWhiteSpace(propertyName) || propertyValue == null || string.IsNullOrWhiteSpace((propertyValue + string.Empty)))
            {
                return True<T>();
            }

            System.Linq.Expressions.Expression? exp = null;
            var p = System.Linq.Expressions.Expression.Parameter(typeof(T), "p");
            var member = System.Linq.Expressions.Expression.PropertyOrField(p, propertyName);
            var constant = System.Linq.Expressions.Expression.Constant(1);

            // 如果是List对象，需要循环转换里面的值
            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyValue.GetType()))
            {
                var guidPropertyValues = new List<Guid>();
                var strPropertyValues = new List<string>();
                foreach (var item in (System.Collections.IEnumerable)propertyValue)
                {
                    if (member.Type.Name.Equals("Guid"))
                    {
                        guidPropertyValues.Add(Guid.Parse(item.ToString() + string.Empty));
                    }
                    else
                    {
                        strPropertyValues.Add(item.ToString() + string.Empty);
                    }
                }

                if (guidPropertyValues.Count > 0)
                {
                    constant = System.Linq.Expressions.Expression.Constant(guidPropertyValues);
                }
                else if (strPropertyValues.Count > 0)
                {
                    constant = System.Linq.Expressions.Expression.Constant(strPropertyValues);
                }

            }
            else
            {
                constant = System.Linq.Expressions.Expression.Constant(propertyValue, member.Type);
            }

            //var value = Convert.ChangeType(propertyValue, Nullable.GetUnderlyingType(member.Type) ?? member.Type, null);//ConvertUtility.ConvertObject(propertyValue, member.Type);//入参值的类型转成实体的字段类型
            //var constant = Expression.Constant(value, member.Type);//创建常数，此方法不支持操作List对象
            //constant = Expression.Constant(newPropertyValues);//创建常数，此方法不支持值类型，因为存的值，跟实体的值类型不一致会报错
            //var constant = Expression.Constant(propertyValue);//创建常数，此方法不支持操作List对象

            switch (eop)
            {
                case ExprOperator.Equal:
                    {
                        exp = System.Linq.Expressions.Expression.Equal(member, constant);
                        break;
                    }

                case ExprOperator.NotEqual:
                    {
                        exp = System.Linq.Expressions.Expression.NotEqual(member, constant);
                        break;
                    }

                case ExprOperator.GreaterThan:
                    {
                        exp = System.Linq.Expressions.Expression.GreaterThan(member, constant);
                        break;
                    }

                case ExprOperator.GreaterThanOrEqual:
                    {
                        exp = System.Linq.Expressions.Expression.GreaterThanOrEqual(member, constant);
                        break;
                    }

                case ExprOperator.LessThan:
                    {
                        exp = System.Linq.Expressions.Expression.LessThan(member, constant);
                        break;
                    }

                case ExprOperator.LessThanOrEqual:
                    {
                        exp = System.Linq.Expressions.Expression.LessThanOrEqual(member, constant);
                        break;
                    }

                case ExprOperator.Contains:
                case ExprOperator.StartsWith:
                case ExprOperator.EndsWith:
                    {
                        var name = eop.ToString();
                        var method = typeof(string).GetMethod(name, new[] { typeof(string) });
                        if (method == null)
                        {
                            throw new MissingMethodException(constant.Type.Name);
                        }
                        exp = System.Linq.Expressions.Expression.Call(member, method, constant);
                        break;
                    }

                case ExprOperator.NotContains:
                case ExprOperator.NotStartsWith:
                case ExprOperator.NotEndsWith:
                    {
                        var name = eop.ToString()[3..];
                        var method = typeof(string).GetMethod(name, new[] { typeof(string) });
                        if (method == null)
                        {
                            throw new MissingMethodException(constant.Type.Name);
                        }
                        exp = System.Linq.Expressions.Expression.Not(System.Linq.Expressions.Expression.Call(member, method, constant));
                        break;
                    }

                case ExprOperator.In:
                    {
                        var method = constant.Type.GetMethod("Contains");
                        if (method == null)
                        {
                            throw new MissingMethodException(constant.Type.Name);
                        }

                        exp = System.Linq.Expressions.Expression.Call(constant, method, member);
                        break;
                    }

                case ExprOperator.NotIn:
                    {
                        var method = constant.Type.GetMethod("Contains");
                        if (method == null)
                        {
                            throw new MissingMethodException(constant.Type.Name);
                        }

                        exp = System.Linq.Expressions.Expression.Not(System.Linq.Expressions.Expression.Call(constant, method, member));
                        break;
                    }

                default:
                    {
                        throw new NotImplementedException(nameof(ExprOperator));
                    }
            }

            return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(exp, p);
        }

    }
}
