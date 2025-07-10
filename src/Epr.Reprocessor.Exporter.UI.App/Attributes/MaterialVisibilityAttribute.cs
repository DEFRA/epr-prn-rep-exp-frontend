namespace Epr.Reprocessor.Exporter.UI.App.Attributes;

/// <summary>
/// Attribute to control the visibility of material items in the UI.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
[ExcludeFromCodeCoverage]
public class MaterialVisibilityAttribute : Attribute
{
    /// <summary>
    /// Whether the material is visible on screen.
    /// </summary>
    public bool IsVisible { get; set; } = true;
}