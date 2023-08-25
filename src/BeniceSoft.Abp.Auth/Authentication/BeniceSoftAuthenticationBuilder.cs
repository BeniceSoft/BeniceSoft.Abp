using Microsoft.AspNetCore.Authentication;

namespace BeniceSoft.Abp.Auth.Authentication;

public class BeniceSoftAuthenticationBuilder
{
    public BeniceSoftAuthenticationBuilder(AuthenticationBuilder authenticationBuilder, AuthOptions authOptions)
    {
        AuthenticationBuilder = authenticationBuilder;
        AuthOptions = authOptions;
    }

    public virtual AuthenticationBuilder AuthenticationBuilder { get; }

    public virtual AuthOptions AuthOptions { get; }
}