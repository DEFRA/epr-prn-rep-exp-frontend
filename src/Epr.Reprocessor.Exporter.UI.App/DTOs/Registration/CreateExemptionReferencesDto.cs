namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

public class CreateExemptionReferencesDto
{
    public RegistrationMaterialDto RegistrationMaterial { get; set; }

    public Guid RegistrationMaterialId { get; set; }

    public List<MaterialExemptionReferenceDto> MaterialExemptionReferences { get; set; }
}
