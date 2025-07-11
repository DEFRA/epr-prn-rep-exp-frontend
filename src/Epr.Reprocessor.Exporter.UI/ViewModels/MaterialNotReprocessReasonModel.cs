using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class MaterialNotReprocessReasonModel
{
    public Guid MaterialId { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Enter the reason you’ll not reprocess this waste")]
    [MaxLength(500, ErrorMessage = "The list must be 500 characters or less")]
    public string? MaterialNotReprocessReason { get; set; }

    public string? MaterialName { get; set; }

    public void MapForView(Guid materialId, string reasonForNotReprocessing, string materialName)
    {
        this.MaterialId = materialId;
        this.MaterialNotReprocessReason = reasonForNotReprocessing;
        this.MaterialName = materialName;
    }
}