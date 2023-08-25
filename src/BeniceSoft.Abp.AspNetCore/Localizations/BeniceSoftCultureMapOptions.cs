using System.Collections.Generic;

namespace BeniceSoft.Abp.AspNetCore.Localizations;

public class BeniceSoftCultureMapOptions
{
    public readonly List<CultureMapInfo> CulturesMaps = new();

    public readonly List<CultureMapInfo> UiCulturesMaps = new();
}