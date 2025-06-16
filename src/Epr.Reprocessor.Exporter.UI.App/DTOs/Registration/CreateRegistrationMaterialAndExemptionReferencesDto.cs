namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

public class CreateRegistrationMaterialAndExemptionReferencesDto
{
    public RegistrationMaterialDto_Temp RegistrationMaterial { get; set; }

    public List<MaterialExemptionReferenceDto> MaterialExemptionReferences { get; set; }
}
