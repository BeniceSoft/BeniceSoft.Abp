namespace BeniceSoft.Abp.Auth;

public class AuthOptions
{
    /// <summary>
    /// AM 地址
    /// </summary>
    public string AmUrl { get; set; }

    /// <summary>
    /// 权限中心地址
    /// </summary>
    public string PermissionCenterUrl { get; set; }

    public string? Authority { get; set; }
    public string? Audience { get; set; }

    public string SecurityKey { get; set; }
}