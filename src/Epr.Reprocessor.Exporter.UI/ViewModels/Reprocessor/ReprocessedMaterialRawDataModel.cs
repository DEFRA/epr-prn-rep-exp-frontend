namespace Epr.Reprocessor.Exporter.UI.ViewModels.Reprocessor;

public class ReprocessedMaterialOutputSummaryModel
{
    public string MaterialName { get; set; }
    public decimal? SentToOtherSiteTonnes { get; set; }
    public decimal? ContaminantTonnes { get; set; }
    public decimal? ProcessLossTonnes { get; set; }
    public decimal? TotalInputTonnes { get; set; }
    public List<ReprocessedMaterialRawDataModel> ReprocessedMaterialsRawData { get; set; }
    public decimal TotalOutputTonnes
    {
        get
        {
            decimal total = (SentToOtherSiteTonnes ?? 0) + (ContaminantTonnes ?? 0) + (ProcessLossTonnes ?? 0);
            if (ReprocessedMaterialsRawData != null && ReprocessedMaterialsRawData.Any())
            {
                total += ReprocessedMaterialsRawData
                    .Where(rm => !string.IsNullOrEmpty(rm.MaterialOrProductName))
                    .Sum(rm => (rm.ReprocessedTonnes??0));
            }

            return total;
        }
    }   
}