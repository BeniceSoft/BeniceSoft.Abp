using System;
using System.Text.Json;

namespace BeniceSoft.Abp.Core.Extensions;

public static class JsonExtensions
{
    public static T? JsonTo<T>(this string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return default;

        try
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        catch(Exception e)
        {
            return default;
        }
    }

    public static string? ToJson<T>(this T? obj, bool isCamelCase = false)
    {
        if (obj is null) return null;

        var options = new JsonSerializerOptions();
        if (isCamelCase)
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        }

        try
        {
            return JsonSerializer.Serialize(obj, options);
        }
        catch
        {
            return null;
        }
    }
}