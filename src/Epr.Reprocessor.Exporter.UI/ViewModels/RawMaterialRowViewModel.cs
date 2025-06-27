using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class RawMaterialRowViewModel
{
    public string RawMaterialName { get; set; }

    //[Range(0, double.MaxValue, ErrorMessage = "Please enter a valid tonnage")]
    public int? Tonnes { get; set; }

}