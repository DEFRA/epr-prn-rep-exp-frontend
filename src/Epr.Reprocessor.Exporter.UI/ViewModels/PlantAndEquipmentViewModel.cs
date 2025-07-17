using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class PlantAndEquipmentViewModel
{
    public string MaterialName { get; set; } = string.Empty;

    [Required(ErrorMessageResourceType = typeof(PlantAndEquipment), ErrorMessageResourceName = "requiredErrorMessage")]
    [MaxLength(500, ErrorMessageResourceType = typeof(PlantAndEquipment), ErrorMessageResourceName = "invalidLengthErrorMessage")]
    public string? PlantEquipmentUsed { get; set; }

    public void MapForView(string materialName, string? plantEquipmentUsed)
    {
        this.MaterialName = materialName;
        this.PlantEquipmentUsed = plantEquipmentUsed;
    }
}
