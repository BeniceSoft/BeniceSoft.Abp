using BeniceSoft.OAuth.DingTalk;
using Microsoft.AspNetCore.Authentication;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to add DingTalk authentication capabilities to an HTTP application pipeline.
/// </summary>
public static class DingTalkAuthenticationExtensions
{
    /// <summary>
    /// Adds <see cref="DingTalkAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables DingTalk authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddDingTalk(this AuthenticationBuilder builder)
    {
        return builder.AddDingTalk(DingTalkAuthenticationDefaults.AuthenticationScheme, options => { });
    }

    /// <summary>
    /// Adds <see cref="DingTalkAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables DingTalk authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="configuration">The delegate used to configure the OpenID 2.0 options.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddDingTalk(
        this AuthenticationBuilder builder,
        Action<DingTalkAuthenticationOptions> configuration)
    {
        return builder.AddDingTalk(DingTalkAuthenticationDefaults.AuthenticationScheme, configuration);
    }

    /// <summary>
    /// Adds <see cref="DingTalkAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables DingTalk authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="scheme">The authentication scheme associated with this instance.</param>
    /// <param name="configuration">The delegate used to configure the DingTalk options.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddDingTalk(
        this AuthenticationBuilder builder,
        string scheme,
        Action<DingTalkAuthenticationOptions> configuration)
    {
        return builder.AddDingTalk(scheme, DingTalkAuthenticationDefaults.DisplayName, configuration);
    }

    /// <summary>
    /// Adds <see cref="DingTalkAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables DingTalk authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="scheme">The authentication scheme associated with this instance.</param>
    /// <param name="caption">The optional display name associated with this instance.</param>
    /// <param name="configuration">The delegate used to configure the DingTalk options.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddDingTalk(
        this AuthenticationBuilder builder,
        string scheme,
        string? caption,
        Action<DingTalkAuthenticationOptions> configuration)
    {
        return builder.AddOAuth<DingTalkAuthenticationOptions, DingTalkAuthenticationHandler>(scheme, caption!, configuration);
    }
}