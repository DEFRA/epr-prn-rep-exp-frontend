namespace Epr.Reprocessor.Exporter.UI.Domain;

/// <summary>
/// Represents details of an exemption for a material.
/// </summary>
[ExcludeFromCodeCoverage]
public class Exemption
{
   public Guid ExternalId { get; set; }

    public int RegistrationMaterialId  { get; set; }

    public string ReferenceNumber { get; set; }
}