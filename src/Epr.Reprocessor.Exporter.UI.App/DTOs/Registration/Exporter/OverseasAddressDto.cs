using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration.Exporter;

[ExcludeFromCodeCoverage]
public class OverseasAddressDto : OverseasAddressBase
{
    public List<OverseasAddressContactDto> OverseasAddressContact { get; set; } = new();
    public List<OverseasAddressWasteCodesDto> OverseasAddressWasteCodes { get; set; } = new();
}