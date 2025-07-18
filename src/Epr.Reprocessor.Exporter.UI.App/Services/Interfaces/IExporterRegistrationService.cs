using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

public interface IExporterRegistrationService
{
    Task SaveOverseasReprocessorAsync(OverseasAddressRequestDto request);
    Task<List<OverseasMaterialReprocessingSiteDto>?> GetOverseasMaterialReprocessingSites(Guid RegistrationMaterialId);
    Task UpsertInterimSitesAsync(SaveInterimSitesRequestDto request);
}