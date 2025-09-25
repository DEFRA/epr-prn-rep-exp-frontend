using System.Text.RegularExpressions;

namespace Epr.Reprocessor.Exporter.UI.App.Constants;

public static class CompiledRegEx
{
    public static readonly Regex TrailingDigits = new(
        @"\d+$",
    RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(50));
}
