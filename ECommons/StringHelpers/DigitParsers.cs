using System.Text.RegularExpressions;

namespace ECommons.StringHelpers;

public static partial class DigitParsers
{
    [GeneratedRegex(@"^[0-9]+", RegexOptions.CultureInvariant)]
    public static partial Regex Number();

    [GeneratedRegex(@"[0-9][0-9,]*", RegexOptions.CultureInvariant)]
    public static partial Regex GroupedNumber();

    [GeneratedRegex(@"^[^\\p{L}]+", RegexOptions.CultureInvariant)]
    public static partial Regex LeadingGlyphs();

    public static int FirstNumber(string text)
    {
        var match = GroupedNumber().Match(text);
        return match.Success && int.TryParse(match.Value.Replace(",", ""), out var value) ? value : 0;
    }

    public static int Digits(string text)
    {
        var match = Number().Match(text);
        return match.Success && int.TryParse(match.Value, out var value) ? value : 0;
    }
}
