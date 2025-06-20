namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

/// <summary>
/// Defines a lookup dto for the details of a singular material including its name and ID.
/// </summary>
[ExcludeFromCodeCoverage]
public record PermitTypeLookupDto
{
    /// <summary>
    /// The name of the permit.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// The id of the permit.
    /// </summary>
    public int Id { get; set; }
}