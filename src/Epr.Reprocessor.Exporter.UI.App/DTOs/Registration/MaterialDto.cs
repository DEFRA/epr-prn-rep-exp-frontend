using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.App.Extensions;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

/// <summary>
/// Represents details of an individual material.
/// </summary>
[ExcludeFromCodeCoverage]
public class MaterialDto
{
    /// <summary>
    /// The name of the material.
    /// </summary>
    public MaterialItem Name { get; set; }

    /// <summary>
    /// The shorthand code for the material.
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// The shorthand code for the material.
    /// </summary>
    public string DisplayText => Name.GetDisplayName();
}