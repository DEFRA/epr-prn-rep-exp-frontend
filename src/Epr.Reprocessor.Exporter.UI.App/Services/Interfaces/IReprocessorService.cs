using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

/// <summary>
/// Defines a contract to manage details related to a reprocessor, encapsulates sub services for registrations and registration materials.
/// </summary>
public interface IReprocessorService
{
    /// <summary>
    /// An accessor that can be used to manage registrations.
    /// </summary>
    public IRegistrationService Registrations { get; }

    /// <summary>
    /// An accessor that can be used to manage registration materials.
    /// </summary>
    public IRegistrationMaterialService RegistrationMaterials { get; }

    /// <summary>
    /// An accessor that can be used to manage materials.
    /// </summary>
    public IMaterialService Materials { get; }

    /// <summary>
    /// An accessor that can be used to manage waste carrier broker dealer references.
    /// </summary>
    public IWasteCarrierBrokerDealerRefService WasteCarrierBrokerDealerService { get; }
}