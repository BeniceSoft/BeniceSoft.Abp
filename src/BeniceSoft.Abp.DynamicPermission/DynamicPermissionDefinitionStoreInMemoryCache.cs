using System.Collections.Concurrent;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.DynamicPermission;

public class DynamicPermissionDefinitionStoreInMemoryCache : ISingletonDependency
{
    private readonly ConcurrentDictionary<string, PermissionDefinition> _permissionDefinitions = new();
    private readonly ConcurrentDictionary<string, PermissionGroupDefinition> _permissionGroupDefinitions = new();

    public void Initialize(string[] policies)
    {
        var context = new PermissionDefinitionContext(null!);
        var group = context.AddGroup("dynamic");
        _permissionGroupDefinitions.TryAdd(group.Name, group);
        foreach (var policy in policies)
        {
            var permissionDefinition = group.AddPermission(policy);
            _permissionDefinitions.TryAdd(permissionDefinition.Name, permissionDefinition);
        }
    }

    public PermissionDefinition? GetPermissionOrNull(string name)
    {
        return _permissionDefinitions.GetOrDefault(name);
    }

    public IReadOnlyList<PermissionDefinition> GetPermissions()
    {
        return _permissionDefinitions.Values.ToList();
    }

    public IReadOnlyList<PermissionGroupDefinition> GetGroups()
    {
        return _permissionGroupDefinitions.Values.ToList();
    }
}