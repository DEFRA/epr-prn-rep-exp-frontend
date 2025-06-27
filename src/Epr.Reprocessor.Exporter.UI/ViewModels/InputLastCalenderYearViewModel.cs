using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class InputLastCalenderYearViewModel
{
    public string PreviousYear { get; set; } = DateTime.Now.AddYears(-1).Year.ToString();

    public string MaterialName { get; set; } = "Steel"; //Need to take from session

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

}