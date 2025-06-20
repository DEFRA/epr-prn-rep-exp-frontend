namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

[ExcludeFromCodeCoverage]
public class MaterialExemptionReferenceDto
{
    public Guid Id { get; set; }

    public Guid RegistrationMaterialId { get; set; }

    public string ReferenceNumber { get; set; }
}
