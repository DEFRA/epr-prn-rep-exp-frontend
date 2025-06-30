using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class InputLastCalenderYearViewModel
{
    public string PreviousYear { get; set; } = DateTime.Now.AddYears(-1).Year.ToString();

    public string MaterialName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter the value.")]
    public int? UkPackagingWaste { get; set; }

    [Required(ErrorMessage = "Please enter the  value")]
    public int? NonUkPackagingWaste { get; set; }

    [Required(ErrorMessage = "Please enter the  value")]
    public int? NonPackagingWaste { get; set; }

    public List<RawMaterialRowViewModel> RawMaterials { get; set; } = new List<RawMaterialRowViewModel>();

    public InputLastCalenderYearViewModel()
    {
        // Initialize with 10 empty rows
        for (int i = 0; i < 10; i++)
        {
            RawMaterials.Add(new RawMaterialRowViewModel());
        }
    }

    public int TotalInputTonnes
    {
        get
        {
            int total = (UkPackagingWaste ?? 0) + (NonUkPackagingWaste ?? 0) + (NonPackagingWaste ?? 0);

            if (RawMaterials != null && RawMaterials.Any())
            {
                total += RawMaterials
                    .Where(rm => !string.IsNullOrWhiteSpace(rm.RawMaterialName) && (rm.Tonnes ?? 0) > 0)
                    .Sum(rm => rm.Tonnes ?? 0);
            }

            return total;
        }
    }

    public void MapForView(RegistrationMaterialDto material)
    {
        this.MaterialName = material.MaterialLookup.Name.GetDisplayName();

    }

}