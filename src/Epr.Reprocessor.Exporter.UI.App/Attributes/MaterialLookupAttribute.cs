namespace Epr.Reprocessor.Exporter.UI.App.Attributes;

/// <summary>
/// Attribute to control the visibility of material items in the UI.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
[ExcludeFromCodeCoverage]
public class MaterialLookupAttribute : Attribute
{
    /// <summary>
    /// Whether the material is visible on screen.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// The value to use for the material item instead of the default.
    /// </summary>
    public string Value { get; set; } = null!;
}