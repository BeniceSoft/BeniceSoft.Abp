using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.DynamicPermission;

[Dependency(ReplaceServices = true)]
public class BeniceSoftDynamicPermissionDefinitionStore(DynamicPermissionDefinitionStoreInMemoryCache cache)
    : IDynamicPermissionDefinitionStore, ITransientDependency
{
    public Task<PermissionDefinition?> GetOrNullAsync(string name)
    {
        return Task.FromResult(cache.GetPermissionOrNull(name));
    }

    public Task<IReadOnlyList<PermissionDefinition>> GetPermissionsAsync()
    {
        return Task.FromResult(cache.GetPermissions());
    }

    public Task<IReadOnlyList<PermissionGroupDefinition>> GetGroupsAsync()
    {
        return Task.FromResult(cache.GetGroups());
    }
}