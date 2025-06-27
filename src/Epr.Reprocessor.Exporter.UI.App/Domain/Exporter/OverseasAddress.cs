using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;

namespace Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;

[ExcludeFromCodeCoverage]
public class OverseasAddress : OverseasAddressBase
{
    public bool IsActive { get; set; }

    public List<OverseasAddressContact> OverseasAddressContact { get; set; } = new();

    public List<OverseasAddressWasteCodes> OverseasAddressWasteCodes { get; set; } = new();
}