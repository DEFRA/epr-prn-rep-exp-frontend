using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class PlantEquipmentUsedViewModel
{
    public string MaterialName { get; set; } = string.Empty;

    [Required(ErrorMessageResourceType = typeof(PlantEquipmentUsed), ErrorMessageResourceName = "requiredErrorMessage")]
    [MaxLength(500, ErrorMessageResourceType = typeof(PlantEquipmentUsed), ErrorMessageResourceName = "invalidLengthErrorMessage")]
    public string? PlantEquipmentUsed { get; set; }

    public void MapForView(RegistrationMaterialDto material)
    {
        this.MaterialName = material.MaterialLookup.Name.GetDisplayName();
    }
}
