namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

[ExcludeFromCodeCoverage]
public class CreateExemptionReferencesDto
{
    public Guid RegistrationMaterialId { get; set; }

    public List<MaterialExemptionReferenceDto> MaterialExemptionReferences { get; set; }
}
