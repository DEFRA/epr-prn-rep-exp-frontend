using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;

[ExcludeFromCodeCoverage]
public class OverseasReprocessingSites
{
    public Guid RegistrationMaterialId { get; set; }

    public List<OverseasAddress>? OverseasAddresses { get; set; }
}