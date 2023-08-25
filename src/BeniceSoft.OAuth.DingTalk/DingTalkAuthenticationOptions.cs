using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using static BeniceSoft.OAuth.DingTalk.DingTalkAuthenticationConstants;

namespace BeniceSoft.OAuth.DingTalk;

/// <summary>
/// Defines a set of options used by <see cref="DingTalkAuthenticationHandler"/>.
/// </summary>
public class DingTalkAuthenticationOptions : OAuthOptions
{
    public DingTalkAuthenticationOptions()
    {
        ClaimsIssuer = DingTalkAuthenticationDefaults.Issuer;
        CallbackPath = new PathString(DingTalkAuthenticationDefaults.CallbackPath);

        AuthorizationEndpoint = DingTalkAuthenticationDefaults.AuthorizationEndpoint;
        UserInformationEndpoint = DingTalkAuthenticationDefaults.UserInformationEndpoint;
        TokenEndpoint = DingTalkAuthenticationDefaults.TokenEndpoint;
        GetByUnionIdEndpoint = DingTalkAuthenticationDefaults.GetByUnionIdEndpoint;
        GetUserInfoByCodeEndpoint = DingTalkAuthenticationDefaults.GetUserInfoByCodeEndpoint;

        Scope.Add("snsapi_login");

        ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "openid");
        ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        ClaimActions.MapJsonKey(ClaimTypes.MobilePhone, "mobile");
        ClaimActions.MapJsonKey(Claims.Nickname, "nick");
        ClaimActions.MapJsonKey(Claims.UnionId, "unionid");
        ClaimActions.MapJsonKey(Claims.OpenId, "openid");
        ClaimActions.MapJsonKey(Claims.MainOrgAuthHighLevel, "main_org_auth_high_level");
        ClaimActions.MapJsonKey(Claims.Avatar, "avatar");
        ClaimActions.MapJsonKey(Claims.Active, "active");
    }

    /// <summary>
    /// Address book language.
    ///     zh_CN: Chinese (default)
    ///     en_US: English
    /// </summary>
    public string Language { get; set; } = "zh_CN";

    /// <summary>
    /// Gets or sets the URL of the UnionId endpoint.
    /// </summary>
    public string GetByUnionIdEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the URL of the UserInfo endpoint.
    /// </summary>
    public string GetUserInfoByCodeEndpoint { get; set; }
}