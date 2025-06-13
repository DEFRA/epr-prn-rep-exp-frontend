using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.App.Extensions;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

/// <summary>
/// Defines a lookup dto for the details of a singular material including its name and ID, this is tied to a <see cref="RegistrationMaterialDto.MaterialLookup"/> which defines the details of the material lookup item that is associated with this registration for a material.
/// </summary>
[ExcludeFromCodeCoverage]
public record MaterialLookupDto
{
    /// <summary>
    /// The name of the material.
    /// </summary>
    public MaterialItem Name { get; set; }

    /// <summary>
    /// The id of the entry, used to tie entries back together.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// The shorthand code for the material.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// The display text for the material to be displayed on screen.
    /// </summary>
    public string DisplayText => Name.GetDisplayName();
}