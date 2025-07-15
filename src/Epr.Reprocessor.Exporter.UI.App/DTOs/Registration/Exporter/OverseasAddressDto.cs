using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration.Exporter;

[ExcludeFromCodeCoverage]
public class OverseasAddressDto : OverseasAddressBase
{
    public string SiteCoordinates { get; set; }
    public List<OverseasAddressContactDto> OverseasAddressContact { get; set; } = new();
    public List<OverseasAddressWasteCodesDto> OverseasAddressWasteCodes { get; set; } = new();
}