namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

public class CreateExemptionReferencesDto
{
    public int RegistrationMaterialId { get; set; }
    
    public List<MaterialExemptionReferenceDto> MaterialExemptionReferences { get; set; }
}
