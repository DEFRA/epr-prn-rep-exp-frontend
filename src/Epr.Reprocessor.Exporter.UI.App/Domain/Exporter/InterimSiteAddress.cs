using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;

[ExcludeFromCodeCoverage]
public class InterimSiteAddress : OverseasAddressBase
{
    public bool IsActive { get; set; }
    public List<OverseasAddressContact> InterimAddressContact { get; set; } = new();
}