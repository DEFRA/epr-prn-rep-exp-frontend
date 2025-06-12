namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

public interface IRegistrationMaterialService
{
    Task CreateRegistrationMaterialAndExemptionReferences(CreateRegistrationMaterialAndExemptionReferencesDto dto);
}
