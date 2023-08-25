using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.Auth.Core;

public interface ICurrentUserPermissionAccessor : ISingletonDependency
{
    IUserPermission? UserPermission { get; set; }
}