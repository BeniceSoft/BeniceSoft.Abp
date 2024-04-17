using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.Authorization.OpenAuthing;

public class OpenAuthingPermissionValueProvider : IPermissionValueProvider, ITransientDependency
{
    // TODO 从 OpenAuthing 获取用户权限列表 再判断是否有权限
    public async Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context)
    {
        return PermissionGrantResult.Granted;
    }

    public Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context)
    {
        throw new NotImplementedException();
    }

    public string Name => nameof(OpenAuthingPermissionValueProvider);
}