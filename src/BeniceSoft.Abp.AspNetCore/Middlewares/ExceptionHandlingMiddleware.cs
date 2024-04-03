using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BeniceSoft.Abp.Core;
using BeniceSoft.Abp.Core.Constants;
using BeniceSoft.Abp.Core.Exceptions;
using BeniceSoft.Abp.Core.Extensions;
using BeniceSoft.Abp.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Http;
using Volo.Abp.Http.Client;
using Volo.Abp.Validation;

namespace BeniceSoft.Abp.AspNetCore.Middlewares;

public class ExceptionHandlingMiddleware : IMiddleware, ITransientDependency
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
            if (context.Response.StatusCode == 401)
            {
                throw new NoAuthorizationException();
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(new EventId(1, Guid.NewGuid().ToString()), exception.GetBaseException(),
                exception.Message);

            // 来自远程服务调用的请求
            if (context.Request.Headers.TryGetValue(BeniceSoftHttpConstant.RequestedFrom, out var requestedFrom) &&
                string.Equals(BeniceSoftHttpConstant.RequestedFromRemoteServiceCall, requestedFrom, StringComparison.OrdinalIgnoreCase))
            {
                await HandleRemoteServiceErrorResponse(context, exception);
                return;
            }

            var responseFeature = context.Features.Get<IHttpResponseFeature>();
            if (responseFeature is { HasStarted: true })
            {
                // Throw: StatusCode cannot be set because the response has already started
                return;
            }

            var result = WarpExceptionToJsonResult(exception);
            if (_env.IsDevelopment())
            {
                result.Exception = exception.Message;
            }
            await HandleResponseAsync(context, result, exception.Message);
        }
    }

    private static async Task HandleRemoteServiceErrorResponse(HttpContext context, Exception exception)
    {
        var errorInfo = new RemoteServiceErrorInfo
        {
            Code = "500",
            Message = exception.Message,
            Data = exception.Data
        };
        var remoteServiceErrorResponse = new RemoteServiceErrorResponse(errorInfo);

        context.Response.Headers.TryAdd(AbpHttpConsts.AbpErrorFormat, bool.TrueString);
        context.Response.ContentType = "application/json;charset=utf-8";
        await context.Response.WriteAsync(remoteServiceErrorResponse.ToJson() ?? string.Empty)
            .ConfigureAwait(false);
        await context.Response.Body.FlushAsync();
    }

    private static async Task HandleResponseAsync(HttpContext context, ResponseResult result, string errorMessage)
    {
        context.Response.StatusCode = 200;
        context.Response.ContentType = "application/json;charset=utf-8";
        await context.Response.WriteAsync(result.ToJson(true) ?? "{\"code\": 500, \"message\": \"系统出现不可预期的错误\"}")
            .ConfigureAwait(false);
        await context.Response.Body.FlushAsync();
    }

    private static ResponseResult WarpExceptionToJsonResult(Exception exception)
    {
        return exception switch
        {
            UserFriendlyException userFriendlyException => new(HttpStatusCode.BadRequest,
                userFriendlyException.Message),
            NoAuthorizationException => new(HttpStatusCode.Unauthorized, "未授权或登录信息已过期"),
            AbpValidationException validationException => new(HttpStatusCode.BadRequest,
                string.Join(';', validationException.ValidationErrors.Select(x => x.ErrorMessage))),
            EntityNotFoundException entityNotFoundException =>
                new(HttpStatusCode.NotFound, $"所操作的对象{entityNotFoundException.Id}不存在"),
            SynchronizationLockException => new(HttpStatusCode.Locked, "资源已被占用，请稍候再试"),
            AbpRemoteCallException remoteCallException => new(HttpStatusCode.ServiceUnavailable,
                $"远程服务不可用({remoteCallException.HttpStatusCode})"),
            _ => new(HttpStatusCode.InternalServerError, "系统出现不可预期的错误")
        };
    }
}

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// 异常处理
    /// </summary>
    /// <param name="app"></param>
    public static void UseBeniceSoftExceptionHandlingMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}