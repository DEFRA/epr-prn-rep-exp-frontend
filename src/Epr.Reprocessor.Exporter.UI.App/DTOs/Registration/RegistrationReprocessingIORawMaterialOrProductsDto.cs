namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

/// <summary>
/// Defines a dto for the Registration Reprocessing Raw Material Or Product
/// </summary>
[ExcludeFromCodeCoverage]
public record RegistrationReprocessingIORawMaterialOrProductsDto
{
    public int RegistrationReprocessingIOId { get; set; }

    public string RawMaterialOrProductName { get; set; }

    public decimal TonneValue { get; set; }

    public bool IsInput { get; set; }
}
