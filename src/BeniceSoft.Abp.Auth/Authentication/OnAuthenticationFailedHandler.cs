using System.Net;
using BeniceSoft.Abp.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.Auth.Authentication;

public class OnAuthenticationFailedHandler : ITransientDependency
{
    private readonly ILogger<OnAuthenticationFailedHandler> _logger;

    public OnAuthenticationFailedHandler(ILogger<OnAuthenticationFailedHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(AuthenticationFailedContext authenticationFailedContext)
    {
        var context = authenticationFailedContext.HttpContext;
        var exception = authenticationFailedContext.Exception;

        _logger.LogWarning(exception, exception.Message);

        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        await context.Response.WriteAsJsonAsync(new ResponseResult(HttpStatusCode.Unauthorized, exception.Message));
        await context.Response.CompleteAsync();
    }
}