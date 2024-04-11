using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using BeniceSoft.Abp.Core.Constants;
using BeniceSoft.Abp.Core.Exceptions;
using BeniceSoft.Abp.Core.Extensions;
using BeniceSoft.Abp.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http;

namespace BeniceSoft.Abp.AspNetCore.Middlewares;

public class ExceptionHandlingMiddleware : ITransientDependency
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly ExceptionHandlingOptions _options;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILoggerFactory loggerFactory,
        IOptions<ExceptionHandlingOptions> options)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<ExceptionHandlingMiddleware>();
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
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

            ResponseResult result;
            HttpStatusCode statusCode;
            if (exception is IKnownException knownException)
            {
                statusCode = _options.KnownExceptionStatusCode;
                result = new ResponseResult(knownException.ErrorCode, knownException.Message, knownException.ErrorData);
            }
            else
            {
                statusCode = _options.DetermineUnknownExceptionStatusCode(exception);
                result = _options.DetermineUnknownExceptionResponseResult(exception);
            }

            await HandleResponseAsync(context, statusCode, result, exception.Message);
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

    private static async Task HandleResponseAsync(HttpContext context, HttpStatusCode statusCode, ResponseResult result, string errorMessage)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json;charset=utf-8";
        const string defaultResponseJsonString = "{\"code\": 500, \"message\": \"Internal server error\"}";
        await context.Response.WriteAsync(result.ToJson(true) ?? defaultResponseJsonString)
            .ConfigureAwait(false);
        await context.Response.Body.FlushAsync();
    }
}

public class ExceptionHandlingOptions
{
    public HttpStatusCode KnownExceptionStatusCode { get; set; }
    public HttpStatusCode DefaultUnknownExceptionStatusCode { get; set; }

    public Func<Exception, HttpStatusCode> DetermineUnknownExceptionStatusCode { get; set; }
    public Func<Exception, ResponseResult> DetermineUnknownExceptionResponseResult { get; set; }

    public ExceptionHandlingOptions()
    {
        KnownExceptionStatusCode = HttpStatusCode.OK;
        DefaultUnknownExceptionStatusCode = HttpStatusCode.InternalServerError;

        DetermineUnknownExceptionStatusCode = _ => DefaultUnknownExceptionStatusCode;
        DetermineUnknownExceptionResponseResult = exception => exception switch
        {
            UnauthorizedException unauthorizedException => new ResponseResult(HttpStatusCode.Unauthorized, unauthorizedException.Message),
            UserFriendlyException userFriendlyException => new ResponseResult(HttpStatusCode.BadRequest, userFriendlyException.Message),
            _ => new ResponseResult((int)DefaultUnknownExceptionStatusCode, "Internal server error")
        };
    }
}

public static class ApplicationBuilderExtensions
{
    public static void UseBeniceSoftExceptionHandlingMiddleware(this IApplicationBuilder app)
    {
        app.UseBeniceSoftExceptionHandlingMiddleware(new());
    }

    public static void UseBeniceSoftExceptionHandlingMiddleware(this IApplicationBuilder app, ExceptionHandlingOptions options)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>(Options.Create(options));
    }
}