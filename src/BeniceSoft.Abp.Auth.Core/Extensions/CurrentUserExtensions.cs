using Volo.Abp.Users;

namespace BeniceSoft.Abp.Auth.Core.Extensions;

public static class CurrentUserExtensions
{
    /// <summary>
    /// 当前用户头像
    /// </summary>
    /// <param name="currentUser"></param>
    /// <returns></returns>
    public static string? GetAvatar(this ICurrentUser currentUser)
    {
        return currentUser.FindClaim(BeniceSoftAuthConstants.ClaimTypes.Avatar).GetStringValue();
    }

    /// <summary>
    /// 获取当前用户角色id集合
    /// </summary>
    /// <param name="currentUser"></param>
    /// <returns></returns>
    public static List<Guid>? GetRoleIds(this ICurrentUser currentUser)
    {
        var claims = currentUser.FindClaims(BeniceSoftAuthConstants.ClaimTypes.RoleId);
        return claims?.Select(x => x.GetGuidValue())
            .Where(x => x.HasValue)
            .Select(x => x!.Value).ToList();
    }
}