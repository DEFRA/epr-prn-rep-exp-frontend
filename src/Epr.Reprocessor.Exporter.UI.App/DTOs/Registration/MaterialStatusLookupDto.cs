using Epr.Reprocessor.Exporter.UI.App.Domain;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

/// <summary>
/// Defines a lookup dto for the details of a materials status.
/// </summary>
[ExcludeFromCodeCoverage]
public record MaterialStatusLookupDto
{
    /// <summary>
    /// The string status for the material.
    /// </summary>
    public MaterialStatus Status { get; set; }

    /// <summary>
    /// The id of the entry, used to tie entries back together.
    /// </summary>
    public int Id { get; set; }
}