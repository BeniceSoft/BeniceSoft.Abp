using System.Security.Claims;
using System.Security.Principal;
using OpenIddict.Abstractions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;

namespace BeniceSoft.Abp.Auth;

[ExposeServices(typeof(ICurrentUser))]
public class BeniceSoftCurrentUser : ICurrentUser, ITransientDependency
{
    private static readonly Claim[] EmptyClaimsArray = Array.Empty<Claim>();

    public virtual bool IsAuthenticated => Id.HasValue;

    public virtual Guid? Id => _principalAccessor.Principal?.FindUserId();

    public virtual string UserName => this.FindClaimValue(OpenIddictConstants.Claims.Username);

    public virtual string Name => this.FindClaimValue(OpenIddictConstants.Claims.Name);

    public virtual string SurName => this.FindClaimValue(OpenIddictConstants.Claims.FamilyName);

    public virtual string PhoneNumber => this.FindClaimValue(OpenIddictConstants.Claims.PhoneNumber);

    public virtual bool PhoneNumberVerified => string.Equals(this.FindClaimValue(OpenIddictConstants.Claims.PhoneNumberVerified), "true", StringComparison.InvariantCultureIgnoreCase);

    public virtual string Email => this.FindClaimValue(OpenIddictConstants.Claims.Email);

    public virtual bool EmailVerified => string.Equals(this.FindClaimValue(OpenIddictConstants.Claims.EmailVerified), "true", StringComparison.InvariantCultureIgnoreCase);

    public virtual Guid? TenantId => _principalAccessor.Principal?.FindTenantId();

    public virtual string[] Roles => FindClaims(OpenIddictConstants.Claims.Role).Select(c => c.Value).Distinct().ToArray();

    private readonly ICurrentPrincipalAccessor _principalAccessor;

    public BeniceSoftCurrentUser(ICurrentPrincipalAccessor principalAccessor)
    {
        _principalAccessor = principalAccessor;
    }

    public virtual Claim? FindClaim(string claimType)
    {
        return _principalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == claimType);
    }

    public virtual Claim[] FindClaims(string claimType)
    {
        return _principalAccessor.Principal?.Claims.Where(c => c.Type == claimType).ToArray() ?? EmptyClaimsArray;
    }

    public virtual Claim[] GetAllClaims()
    {
        return _principalAccessor.Principal?.Claims.ToArray() ?? EmptyClaimsArray;
    }

    public virtual bool IsInRole(string roleName)
    {
        return Roles.Any(r => r == roleName);
    }
}