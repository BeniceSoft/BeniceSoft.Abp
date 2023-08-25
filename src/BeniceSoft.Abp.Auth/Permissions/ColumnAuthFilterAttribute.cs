using System.Collections.Concurrent;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using BeniceSoft.Abp.Auth.Core;
using BeniceSoft.Abp.Auth.Core.Models;
using BeniceSoft.Abp.Auth.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application.Dtos;

namespace BeniceSoft.Abp.Auth.Permissions;

/// <summary>
/// 列权限过滤器
/// </summary>
public class ColumnAuthFilterAttribute : ActionFilterAttribute
{
    private readonly ConcurrentDictionary<Type, PropertyInfo[]> _dic = new();

    /// <summary>
    /// Action执行后
    /// </summary>
    /// <param name="context"></param>
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        base.OnActionExecuted(context);

        if (context.Result is not ObjectResult data)
        {
            return;
        }

        var myJson = data.Value;
        var subObject = myJson?.GetType();
        if (subObject == null)
        {
            return;
        }

        // 获取当前请求用户的列权限配置
        var currentUserPermissionAccessor = context.HttpContext.RequestServices
            .GetRequiredService<ICurrentUserPermissionAccessor>();
        var columnCfg = currentUserPermissionAccessor.UserPermission?.ColumnPermissions;
        if (!(columnCfg?.Any() ?? false))
        {
            return;
        }

        var dic = columnCfg.ToDictionary(c => c.TableName + "." + c.ColumnName);
        //var parentObject = typeof(BaseResponse);
        if (subObject.IsAssignableTo(typeof(BaseResponse)))
        {
            var prm = Expression.Parameter(subObject, "data");
            var exp = DynamicExpressionParser.ParseLambda(new[] { prm }, null, "data.Data");
            var tData = exp.Compile().DynamicInvoke(myJson); // 转成 MyJson.Data 对象
            if (tData != null)
            {
                FilterColumns(tData, dic);
            }
        }
        else if (subObject.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IListResult<>)))
        {
            var prm = Expression.Parameter(subObject, "data");
            var exp = DynamicExpressionParser.ParseLambda(new[] { prm }, null, "data.Items");
            var tData = exp.Compile().DynamicInvoke(myJson); // 转成 PagedResultDto.Items 对象
            if (tData != null)
            {
                FilterColumns(tData, dic);
            }
        }
        else
        {
            var prm = Expression.Parameter(subObject, "data");
            var exp = DynamicExpressionParser.ParseLambda(new[] { prm }, null, "data");
            var tData = exp.Compile().DynamicInvoke(myJson); // 转成 Data 对象
            if (tData != null)
            {
                FilterColumns(tData, dic);
            }
        }
    }

    private void FilterColumns(object? tData, Dictionary<string, ColumnPermission> dic)
    {
        if (tData is null)
        {
            return;
        }

        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(tData?.GetType()))
        {
            foreach (var item in (System.Collections.IEnumerable)tData)
            {
                var itemType = item.GetType();
                if (!itemType.IsClass)
                {
                    continue; //防止传入的对象是List<object>,需要进行循环查找下一个类型是class的item
                }

                var props = _dic.GetOrAdd(itemType, () => itemType.GetProperties());

                MaskedAttrValue(item, props, dic);
            }
        }
        else
        {
            var propInfos = _dic.GetOrAdd(tData.GetType(), () => tData?.GetType().GetProperties() ?? new PropertyInfo[0]);
            MaskedAttrValue(tData, propInfos, dic);
        }
    }

    /// <summary>
    /// 根据列权限配置，隐藏字段的真实值
    /// </summary>
    /// <param name="obj">dto对象本身</param>
    /// <param name="propertyInfos">DTO对象的所有属性集合</param>
    /// <param name="keyValuePairs">列权限配置项</param>
    private void MaskedAttrValue(object obj, PropertyInfo[] propertyInfos, Dictionary<string, ColumnPermission> keyValuePairs)
    {
        foreach (var prop in propertyInfos)
        {
            if (!prop.PropertyType.CurrentTypeIsSimpleType())
            {
                var propertyValue = prop.GetValue(obj);
                if (propertyValue is not null)
                {
                    FilterColumns(propertyValue, keyValuePairs);
                }

                continue;
            }

            var attr = prop.GetCustomAttribute<ColumnAuthDescAttribute>(false);
            if (attr == null)
            {
                continue;
            }

            var attrVal = attr.Description;
            if (keyValuePairs.ContainsKey(attrVal))
            {
                var columnPermission = keyValuePairs[attrVal]; //配置的字段是否显示
                if (!columnPermission.IsDisplay)
                {
                    //var t = obj.GetType();
                    //object value = null;
                    //if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                    //{
                    //    if (string.IsNullOrEmpty(attr.MaskedValue))
                    //    {
                    //        value = null;
                    //    }
                    //    else
                    //    {
                    //       value = Convert.ChangeType(attr.MaskedValue, t.GetGenericArguments()[0]);
                    //    }
                    //}
                    //else
                    //{
                    //    value = Convert.ChangeType(attr.MaskedValue, t);
                    //}

                    prop.SetValue(obj, null, null);
                }
            }
        }
    }
}