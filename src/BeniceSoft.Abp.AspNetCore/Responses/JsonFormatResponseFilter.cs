using System.Reflection;
using BeniceSoft.Abp.Core.Attributes;
using BeniceSoft.Abp.Core.Constants;
using BeniceSoft.Abp.Core.Extensions;
using BeniceSoft.Abp.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.AspNetCore.Responses;

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
        if (isIgnore) return;

        var wrappedResult = context.Result switch
        {
            ObjectResult objectResult => WarpMyJsonResult(objectResult.Value),
            StatusCodeResult statusCodeResult => WarpMyJsonResult(new ResponseResult(statusCodeResult.StatusCode, string.Empty)),
            EmptyResult => WarpMyJsonResult(new ResponseResult()),
            _ => null
        };
        if (wrappedResult is not null)
        {
            context.Result = wrappedResult;
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