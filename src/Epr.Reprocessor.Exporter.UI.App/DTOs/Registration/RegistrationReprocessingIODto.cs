namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

/// <summary>
/// Defines a dto for the Registration Reprocessing.
/// </summary>
[ExcludeFromCodeCoverage]
public record RegistrationReprocessingIODto
{
    public Guid ExternalId { get; set; }

    public Guid RegistrationMaterialId { get; set; }

    public bool ReprocessingPackagingWasteLastYearFlag { get; set; }

    public decimal UKPackagingWasteTonne { get; set; }

    public decimal NonUKPackagingWasteTonne { get; set; }

    public decimal NotPackingWasteTonne { get; set; }

    public decimal SenttoOtherSiteTonne { get; set; }

    public decimal ContaminantsTonne { get; set; }

    public decimal ProcessLossTonne { get; set; }

    public string? PlantEquipmentUsed { get; set; }

    public decimal TotalInputs { get; set; }

    public decimal TotalOutputs { get; set; }

    public string? TypeOfSuppliers { get; set; }

    public List<RegistrationReprocessingIORawMaterialOrProductsDto> RegistrationReprocessingIORawMaterialOrProducts { get; set; }
}
