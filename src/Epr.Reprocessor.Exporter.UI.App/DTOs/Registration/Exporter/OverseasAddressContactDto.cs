using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration.Exporter;

[ExcludeFromCodeCoverage]
public class OverseasAddressContactDto
{
    [MaxLength(100)]
    public required string FullName { get; set; }
    [MaxLength(100)]
    public required string Email { get; set; }
    [MaxLength(25)]
    public required string PhoneNumber { get; set; }
}