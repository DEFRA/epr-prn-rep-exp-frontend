namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

[ExcludeFromCodeCoverage]
public class MaterialExemptionReferenceDto
{
    public Guid ExternalId { get; set; }

    public int RegistrationMaterialId { get; set; }

    public string ReferenceNumber { get; set; }
}
