using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;

[ExcludeFromCodeCoverage]
public class InterimSiteAddress : OverseasAddressBase
{
    public bool IsActive { get; set; }
    public Guid ParentOverseasAddressId { get; init; }
    public required string ParentOverseasAddressOrganisationName { get; init; }
    public required string ParentOverseasAddressAddressLine1 { get; init; }
    public List<OverseasAddressContact> InterimAddressContact { get; set; } = new();
}