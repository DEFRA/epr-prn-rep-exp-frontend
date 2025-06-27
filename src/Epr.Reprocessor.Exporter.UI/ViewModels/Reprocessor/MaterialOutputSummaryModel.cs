using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Reprocessor;

public class MaterialOutputSummaryModel
{
    
    public string MaterialName { get; set; }
    [Required(ErrorMessage = "This field is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "SentToOtherSiteTonnes must be greater than zero.")]
    public int SentToOtherSiteTonnes { get; set; }
    [Required(ErrorMessage = "This field is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "ContaminantTonnes must be greater than zero.")]
    public int ContaminantTonnes { get; set; }
    [Required(ErrorMessage = "This field is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "ProcessLossTonnes must be greater than zero.")]
    public int ProcessLossTonnes { get; set; }   
    public int TotalInputTonnes { get; set; }
    public int TotalOutputTonnes
    {
        get
        {
            int total = SentToOtherSiteTonnes + ContaminantTonnes + ProcessLossTonnes;

            if (ReprocessedMaterials != null && ReprocessedMaterials.Any())
            {
                total += ReprocessedMaterials.Sum(rm => rm.ReprocessedTonnes);
            }

            return total;
        }
    }
    public List<ReprocessedMaterial> ReprocessedMaterials { get; set; }
}