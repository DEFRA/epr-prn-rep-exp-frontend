using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class PlantAndEquipmentViewModel
{
    public string MaterialName { get; set; } = string.Empty;

    [Required(ErrorMessageResourceType = typeof(PlantAndEquipment), ErrorMessageResourceName = "requiredErrorMessage")]
    [MaxLength(500, ErrorMessageResourceType = typeof(PlantAndEquipment), ErrorMessageResourceName = "invalidLengthErrorMessage")]
    public string? PlantEquipmentUsed { get; set; }

    public void MapForView(RegistrationMaterialDto material)
    {
        this.MaterialName = material.MaterialLookup.Name.GetDisplayName();
        this.PlantEquipmentUsed = material.RegistrationReprocessingIO.PlantEquipmentUsed;
    }
}
