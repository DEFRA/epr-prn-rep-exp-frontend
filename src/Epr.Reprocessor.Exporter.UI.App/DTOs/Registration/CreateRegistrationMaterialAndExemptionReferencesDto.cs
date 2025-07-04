namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

[ExcludeFromCodeCoverage]
public class CreateRegistrationMaterialAndExemptionReferencesDto
{
    public RegistrationMaterialDto RegistrationMaterial { get; set; }

    public List<MaterialExemptionReferenceDto> MaterialExemptionReferences { get; set; }
}
