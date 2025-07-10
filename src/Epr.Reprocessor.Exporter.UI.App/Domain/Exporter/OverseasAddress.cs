using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;

public class OverseasAddress: OverseasAddressBase
{
    public bool IsActive { get; set; }

    [MaxLength(100)]
    public required string SiteCoordinates { get; set; }
    public List<OverseasAddressContact> OverseasAddressContact { get; set; } = new();
    public List<OverseasAddressWasteCodes> OverseasAddressWasteCodes { get; set; } = new();

}