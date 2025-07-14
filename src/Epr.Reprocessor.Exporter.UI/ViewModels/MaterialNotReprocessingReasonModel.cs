using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class MaterialNotReprocessingReasonModel
{
    public Guid MaterialId { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Enter the reason you’ll not reprocess this waste")]
    [MaxLength(500, ErrorMessage = "The list must be 500 characters or less")]
    public string? MaterialNotReprocessingReason { get; set; }

    public string? MaterialName { get; set; }

    public void MapForView(Guid materialId, string reasonForNotReprocessing, string materialName)
    {
        this.MaterialId = materialId;
        this.MaterialNotReprocessingReason = reasonForNotReprocessing;
        this.MaterialName = materialName;
    }
}