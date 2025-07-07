using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

public interface IExporterRegistrationService
{
    Task SaveOverseasReprocessorAsync(OverseasAddressRequestDto request);
}