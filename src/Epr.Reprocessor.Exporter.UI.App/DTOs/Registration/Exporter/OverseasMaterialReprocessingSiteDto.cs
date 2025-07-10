using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;

[ExcludeFromCodeCoverage]
public class OverseasMaterialReprocessingSiteDto
{
    public Guid Id { get; init; }
    public Guid OverseasAddressId { get; init; }
    public required OverseasAddressBaseDto OverseasAddress { get; init; }
    public List<InterimSiteAddressDto>? InterimSiteAddresses { get; set; } = new();
}