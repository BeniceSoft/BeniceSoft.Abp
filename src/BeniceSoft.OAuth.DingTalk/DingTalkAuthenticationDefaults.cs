using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace BeniceSoft.OAuth.DingTalk;

/// <summary>
/// Default values for DingTalk authentication.
/// </summary>
public static class DingTalkAuthenticationDefaults
{
    /// <summary>
    /// Default value for <see cref="AuthenticationScheme.Name"/>.
    /// </summary>
    public const string AuthenticationScheme = "DingTalk";

    /// <summary>
    /// Default value for <see cref="AuthenticationScheme.DisplayName"/>.
    /// </summary>
    public const string DisplayName = "DingTalk";

    /// <summary>
    /// Default value for <see cref="AuthenticationSchemeOptions.ClaimsIssuer"/>.
    /// </summary>
    public const string Issuer = "DingTalk";

    /// <summary>
    /// Default value for <see cref="RemoteAuthenticationOptions.CallbackPath"/>.
    /// </summary>
    public const string CallbackPath = "/signin-dingtalk";

    /// <summary>
    /// Default value for <see cref="OAuthOptions.AuthorizationEndpoint"/>.
    /// </summary>
    public const string AuthorizationEndpoint = "https://oapi.dingtalk.com/connect/qrconnect";

    /// <summary>
    /// Default value for <see cref="OAuthOptions.TokenEndpoint"/>.
    /// </summary>
    public const string TokenEndpoint = "https://oapi.dingtalk.com/gettoken";

    /// <summary>
    /// Default value for <see cref="OAuthOptions.UserInformationEndpoint"/>.
    /// </summary>
    public const string UserInformationEndpoint = "https://oapi.dingtalk.com/topapi/v2/user/get";

    /// <summary>
    /// Default value for <see cref="DingTalkAuthenticationOptions.GetByUnionIdEndpoint"/>.
    /// </summary>
    public const string GetByUnionIdEndpoint = "https://oapi.dingtalk.com/topapi/user/getbyunionid";

    /// <summary>
    /// Default value for <see cref="DingTalkAuthenticationOptions.GetUserInfoByCodeEndpoint"/>.
    /// </summary>
    public const string GetUserInfoByCodeEndpoint = "https://oapi.dingtalk.com/sns/getuserinfo_bycode";
}