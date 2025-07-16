namespace Epr.Reprocessor.Exporter.UI.App.Domain;

/// <summary>
/// Represents details of an exemption for a material.
/// </summary>
[ExcludeFromCodeCoverage]
public class Exemption
{
    /// <summary>
    /// The reference number for this exemption.
    /// </summary>
    public string ReferenceNumber { get; set; } = null!;
}