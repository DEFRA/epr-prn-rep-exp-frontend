using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class MaterialNotReprocessReasonModel
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Enter the reason you’ll not reprocess this waste")]
    [MaxLength(500, ErrorMessage = "The list must be 500 characters or less")]
    public string? ReasonForNotReprocessing { get; set; }

    public string? MaterialName { get; set; }

    public void MapForView(string reasonForNotReprocessing, string materialName)
    {
        this.ReasonForNotReprocessing = reasonForNotReprocessing;
        this.MaterialName = materialName;
    }
}