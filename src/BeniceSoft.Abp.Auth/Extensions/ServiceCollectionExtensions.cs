using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BeniceSoft.Abp.Auth.Authentication;
using BeniceSoft.Abp.Auth.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace BeniceSoft.Abp.Auth.Extensions;

public static class ServiceCollectionExtensions
{
    public static BeniceSoftAuthenticationBuilder AddBeniceSoftAuthentication(this IServiceCollection services)
    {
        var authOptions = new AuthOptions();
        services.GetConfiguration().GetSection("Auth").Bind(authOptions);
        authOptions = services.ExecutePreConfiguredActions(authOptions);
        services.AddSingleton(_ => authOptions);

        // default scheme is Bearer
        var builder = services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
                {
                    options.Audience = authOptions.Audience;
                    options.Authority = authOptions.Authority;
                    options.IncludeErrorDetails = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        RequireExpirationTime = true,
                        RequireAudience = false,
                        SignatureValidator = (token, _) => new JsonWebToken(token)
                    };
                    options.Events = new JwtBearerEvents()
                    {
                        OnAuthenticationFailed = async authenticationFailedContext =>
                        {
                            await authenticationFailedContext.HttpContext.RequestServices
                                .GetRequiredService<OnAuthenticationFailedHandler>()
                                .HandleAsync(authenticationFailedContext);
                        }
                    };
                }
            );

        return new BeniceSoftAuthenticationBuilder(builder, authOptions);
    }


    public static IServiceCollection AddBeniceSoftAuthorization(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IAuthorizationHandler, BeniceSoftAuthorizationHandler>();
        services.AddAuthorization(options =>
        {
            // if (!options.DefaultPolicy.Requirements.OfType<UfxAuthorizationRequirement>().Any())
            // {
            //     options.DefaultPolicy = new AuthorizationPolicyBuilder()
            //         .AddRequirements(new UfxAuthorizationRequirement()).Build();
            // }
        });
        return services;
    }
}