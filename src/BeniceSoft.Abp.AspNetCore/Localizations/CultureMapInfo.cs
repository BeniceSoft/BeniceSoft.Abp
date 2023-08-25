using System.Collections.Generic;

namespace BeniceSoft.Abp.AspNetCore.Localizations;

public class CultureMapInfo
{
    public string TargetCulture { get; set; }

    public List<string> SourceCultures { get; set; } = new();
}