using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace BeniceSoft.Abp.Auth.Authorization;

/// <summary>
/// 授权处理
/// </summary>
public class BeniceSoftAuthorizationHandler : AuthorizationHandler<BeniceSoftAuthorizationRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BeniceSoftAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BeniceSoftAuthorizationRequirement requirement)
    {
        if (context.PendingRequirements.All(r => !(r is BeniceSoftAuthorizationRequirement)))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        // var currentUser = _httpContextAccessor.HttpContext?.Features.Get<ICurrentUser>();
        // if (currentUser is null || !currentUser.IsAuthenticated)
        // {
        //     context.Fail();
        //     return Task.CompletedTask;
        // }
        

        context.Succeed(requirement);

        return Task.CompletedTask;
    }
}