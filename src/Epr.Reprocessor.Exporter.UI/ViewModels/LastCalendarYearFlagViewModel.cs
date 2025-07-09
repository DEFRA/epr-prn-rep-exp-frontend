using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.Resources.Views.ReprocessingInputsAndOutputs;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class LastCalendarYearFlagViewModel
{
    public string TypeOfWaste { get; set; } = string.Empty;

    public int Year { get; set; }

    [Required(ErrorMessageResourceType = typeof(LastCalendarYearFlag), ErrorMessageResourceName = "Error")]
    public bool? ReprocessingPackagingWasteLastYearFlag { get; set; }
}