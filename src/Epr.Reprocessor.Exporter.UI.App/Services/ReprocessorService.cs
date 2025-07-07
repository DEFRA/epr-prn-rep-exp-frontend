using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

/// <summary>
/// Implementation for <see cref="IReprocessorService"/> that encapsulates sub services for registrations and registration materials.
/// </summary>
/// <param name="registrationService">Provides access to a service to manage registrations.</param>
/// <param name="registrationMaterials">Provides access to a service to manage registration materials.</param>
/// <param name="materials">Provides access to a service to manage materials.</param>
public class ReprocessorService(
    IRegistrationService registrationService,
    IRegistrationMaterialService registrationMaterials,
    IMaterialService materials, 
    IWasteCarrierBrokerDealerRefService wasteCarrierBrokerDealerRef)
    : IReprocessorService
{
    /// <inheritdoc />
    public IRegistrationService Registrations { get; } = registrationService;

    /// <inheritdoc />
    public IRegistrationMaterialService RegistrationMaterials { get; } = registrationMaterials;

    /// <inheritdoc />
    public IMaterialService Materials { get; } = materials;

    /// <inheritdoc />
    public IWasteCarrierBrokerDealerRefService WasteCarrierBrokerDealerService { get; } = wasteCarrierBrokerDealerRef;
}