using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration.Exporter;

[ExcludeFromCodeCoverage]
public class OverseasAddressDto
{
    public Guid Id { get; set; }
    public required string OrganisationName { get; set; }
    public int CountryId { get; set; }
    [MaxLength(100)]
    public required string AddressLine1 { get; set; }
    [MaxLength(100)]
    public required string AddressLine2 { get; set; }
    [MaxLength(70)]
    public required string CityorTown { get; set; }
    [MaxLength(70)]
    public required string StateProvince { get; set; }
    [MaxLength(20)]
    public required string PostCode { get; set; }
    [MaxLength(100)]
    public required string SiteCoordinates { get; set; }

    public List<OverseasAddressContactDto> OverseasAddressContact { get; set; } = new();
    public List<OverseasAddressWasteCodesDto> OverseasAddressWasteCodes { get; set; } = new();
}