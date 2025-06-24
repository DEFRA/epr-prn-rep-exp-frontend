using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;

public class OverseasReprocessingSites
{
    /// <summary>
    /// The unique identifier for the material entry.
    /// </summary>
    public Guid RegistrationMaterialId { get; set; }

    /// <summary>
    /// Details of the Overseas Address
    /// </summary>
    public List<OverseasAddress>? OverseasAddresses { get; set; }

}