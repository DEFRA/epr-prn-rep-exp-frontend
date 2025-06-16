namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

/// <summary>
/// Defines a lookup dto for the details of a singular material including its name and ID.
/// </summary>
[ExcludeFromCodeCoverage]
public record MaterialLookupDto
{
    /// <summary>
    /// The name of the material.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// The id of the entry, used to tie entries back together.
    /// </summary>
    public int Id { get; set; }
}