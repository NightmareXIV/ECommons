﻿using static ECommons.LanguageHelpers.Localization;

namespace ECommons.LanguageHelpers;
#nullable disable

public static class LocalizationExtensions
{
    public static string Loc(this string s)
    {
        if (CurrentLocalization.TryGetValue(s, out var locs) && locs != "" && locs != null)
        {
            return locs;
        }
        else if (Localization.Logging)
        {
            CurrentLocalization[s] = "";
        }
        return s;
    }

    public static string Loc(this string s, params object[] values)
    {
        if (CurrentLocalization.TryGetValue(s, out var locs) && locs != "" && locs != null)
        {
            s = locs;
        }
        else if (Localization.Logging)
        {
            CurrentLocalization[s] = "";
        }
        foreach (var x in values)
        {
            s = s.ReplaceFirst(PararmeterSymbol, x.ToString());
        }
        return s;
    }
}
