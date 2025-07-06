using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class InputsForLastCalendarYearViewModel
{
    public string PreviousYear { get; set; } = DateTime.Now.AddYears(-1).Year.ToString();

    public string MaterialName { get; set; } = string.Empty;

    public string? UkPackagingWaste { get; set; }

    public string? NonUkPackagingWaste { get; set; }

    public string? NonPackagingWaste { get; set; }

    public List<RawMaterialRowViewModel> RawMaterials { get; set; } = new List<RawMaterialRowViewModel>();

    public InputsForLastCalendarYearViewModel()
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
            int uk = int.TryParse(UkPackagingWaste, out var ukVal) ? ukVal : 0;
            int nonUk = int.TryParse(NonUkPackagingWaste, out var nonUkVal) ? nonUkVal : 0;
            int nonPackaging = int.TryParse(NonPackagingWaste, out var nonPackagingVal) ? nonPackagingVal : 0;

            int total = uk + nonUk + nonPackaging;

            if (RawMaterials != null && RawMaterials.Count > 0)
            {
                total += RawMaterials
                    .Where(rm => !string.IsNullOrWhiteSpace(rm.RawMaterialName) && !string.IsNullOrWhiteSpace(rm.Tonnes))
                    .Sum(rm => int.TryParse(rm.Tonnes, out var tonnes) ? tonnes : 0);
            }

            return total;
        }            
    }

    public void MapForView(RegistrationMaterialDto material)
    {
        this.MaterialName = material.MaterialLookup.Name.GetDisplayName();

    }

}