namespace Epr.Reprocessor.Exporter.UI.App.Enums;

/// <summary>
/// Defines extensions for <see cref="MaterialItem"/>.
/// </summary>
public static class MaterialItemExtensions
{
    /// <summary>
    /// Attempts to parse to <see cref="MaterialItem"/> taking into account a specific quirk for Paper/Board, probably should make this all const values instead of an enum to avoid a const parsing issue.
    /// </summary>
    /// <param name="input">The input to parse.</param>
    /// <returns>The parsed value.</returns>
    public static MaterialItem GetMaterialName(string input) 
        => string.Equals(input, "Paper/Board", StringComparison.InvariantCultureIgnoreCase) ? MaterialItem.Paper : Enum.Parse<MaterialItem>(input);
}