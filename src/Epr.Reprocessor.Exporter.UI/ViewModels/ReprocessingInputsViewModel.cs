using Epr.Reprocessor.Exporter.UI.Extensions;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class ReprocessingInputsViewModel
{
    public string PreviousYear { get; set; } = DateTime.Now.AddYears(-1).Year.ToString();

    public string MaterialName { get; set; } = string.Empty;

    public string? UkPackagingWaste { get; set; }

    public string? NonUkPackagingWaste { get; set; }

    public string? NonPackagingWaste { get; set; }

    public bool InputsLastCalendarYearFlag { get; set; }

    public List<RawMaterialRowViewModel> RawMaterials { get; set; } = new List<RawMaterialRowViewModel>();

    public ReprocessingInputsViewModel()
    {
        // Initialize with 10 empty rows
        for (int i = 0; i < 10; i++)
        {
            RawMaterials.Add(new RawMaterialRowViewModel());
        }
    }

    public int? TotalInputTonnes
    {
        get
        {
            int total = UkPackagingWaste.ConvertToInt() + NonUkPackagingWaste.ConvertToInt() + NonPackagingWaste.ConvertToInt();

            if (RawMaterials != null && RawMaterials.Count > 0)
            {
                total += RawMaterials
                    .Where(rm => !string.IsNullOrWhiteSpace(rm.RawMaterialName) && !string.IsNullOrWhiteSpace(rm.Tonnes))
                    .Sum(rm => rm.Tonnes.ConvertToInt());
            }
            return total;
        }
    }

    public void MapForView(RegistrationMaterialDto material)
    {
        this.MaterialName = material.MaterialLookup.Name.GetDisplayName();
    }
}