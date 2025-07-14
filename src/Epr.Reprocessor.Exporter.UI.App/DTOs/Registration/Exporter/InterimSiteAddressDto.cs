using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration.Exporter;

[ExcludeFromCodeCoverage]
public class InterimSiteAddressDto : OverseasAddressBase
{
    public Guid? Id { get; init; }
    public List<OverseasAddressContactDto> InterimAddressContact { get; set; } = new();
}