using BeniceSoft.Abp.Auth.Core;
using Microsoft.AspNetCore.Http;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.Auth.Permissions;

public interface IUserPermissionFactory : IDisposable, ISingletonDependency
{
    Task<IUserPermission> CreateAsync(Guid userId, HttpContext httpContext);
}