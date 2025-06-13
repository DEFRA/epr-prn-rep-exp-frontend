namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

[ExcludeFromCodeCoverage]
public class CreateMaterialExemptionReferenceDto
{
    public int RegistrationMaterialId { get; set; }

    public List<MaterialExemptionReferenceDto> MaterialExemptionReferences { get; set; }
}
