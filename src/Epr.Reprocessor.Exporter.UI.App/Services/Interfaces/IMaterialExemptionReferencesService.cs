namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

public interface IMaterialExemptionReferencesService
{
    Task<bool> CreateMaterialExemptionReferences(int registrationMaterialId, List<MaterialExemptionReferenceDto> exemptions);
}
