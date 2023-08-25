using System.Reflection;
using BeniceSoft.Abp.Core.Attributes;
using BeniceSoft.Abp.Core.Constants;
using BeniceSoft.Abp.Core.Extensions;
using BeniceSoft.Abp.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.AspNetCore.Filters;

/// <summary>
/// 统一响应格式化
/// </summary>
public class JsonFormatResponseFilter : IActionFilter, ITransientDependency
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        var isIgnore = context.ActionDescriptor
                           .GetMethodInfo()
                           .GetCustomAttribute<IgnoreJsonFormatAttribute>() is not null ||
                       context.HttpContext.Request.Headers.ContainsKey(BeniceSoftHttpConstant.IgnoreJsonFormat);
        if (isIgnore)
        {
            return;
        }

        var actionResult = context.Result;
        if (actionResult is ObjectResult objectResult)
        {
            context.Result = WarpMyJsonResult(objectResult.Value);
        }
        else if (actionResult is EmptyResult)
        {
            context.Result = WarpMyJsonResult(new ResponseResult());
        }
    }

    private JsonResult WarpMyJsonResult(object? originValue)
    {
        if (originValue is ResponseResult)
        {
            return new JsonResult(originValue);
        }

        return new JsonResult(originValue.ToSucceed());
    }
}