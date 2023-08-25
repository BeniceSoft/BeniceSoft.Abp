using BeniceSoft.Abp.Auth.Permissions;
using Microsoft.AspNetCore.Builder;

namespace BeniceSoft.Abp.Auth.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseBeniceSoftAuthentication(this IApplicationBuilder app)
    {
        app.UseAuthentication();

        return app;
    }

    public static IApplicationBuilder UseBeniceSoftAuthorization(this IApplicationBuilder app)
    {
        app.UseAuthorization();

        return app;
    }

    /// <summary>
    /// UseUserPermission 必须在 UseBeniceSoftAuthentication、UseBeniceSoftAuthorization 之后 <br/>
    /// 会在请求开始初始化用户权限数据 <br/>
    /// 后续可以通过 <see cref="BeniceSoft.Abp.Auth.Core.ICurrentUserPermissionAccessor"/> 获取到当前用户的权限数据
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseBeniceSoftUserPermission(this IApplicationBuilder app)
    {
        app.UseMiddleware<PermissionMiddleware>();

        return app;
    }
}