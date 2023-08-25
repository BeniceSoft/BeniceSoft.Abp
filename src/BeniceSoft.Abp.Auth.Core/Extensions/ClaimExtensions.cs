using System.Security.Claims;

namespace BeniceSoft.Abp.Auth.Core.Extensions;

public static class ClaimExtensions
{
    public static string? GetStringValue(this Claim? claim)
    {
        return claim?.Value;
    }

    public static int? GetIntValue(this Claim? claim)
    {
        var value = claim?.Value;
        if (!string.IsNullOrEmpty(value) &&
            int.TryParse(value, out var intValue))
        {
            return intValue;
        }

        return default;
    }
    
    public static Guid? GetGuidValue(this Claim? claim)
    {
        var value = claim?.Value;
        if (!string.IsNullOrEmpty(value) &&
            Guid.TryParse(value, out var guidValue))
        {
            return guidValue;
        }

        return default;
    }
}