namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

[ExcludeFromCodeCoverage]
public class RegistrationMaterialDto
{
    public Guid Id { get; set; }
    public Guid RegistrationId { get; set; }
    public required string MaterialName { get; set; }
    public int MaterialId { get; set; }
    public int StatusId { get; set; }
    public int PermitTypeId { get; set; }
    public decimal PPCReprocessingCapacityTonne { get; set; }
    public decimal WasteManagementReprocessingCapacityTonne { get; set; }
    public decimal InstallationReprocessingTonne { get; set; }
    public decimal EnvironmentalPermitWasteManagementTonne { get; set; }
    public decimal MaximumReprocessingCapacityTonne { get; set; }
    public bool IsMaterialRegistered { get; set; }
    public DateTime CreatedDate { get; set; }
}