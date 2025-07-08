namespace Epr.Reprocessor.Exporter.UI.ViewModels.Reprocessor;

public class ReprocessedMaterialOutputSummaryModel
{
    public string MaterialName { get; set; }
    public string? SentToOtherSiteTonnes { get; set; }
    public string? ContaminantTonnes { get; set; }
    public string? ProcessLossTonnes { get; set; }
    public List<ReprocessedMaterialRawDataModel> ReprocessedMaterialsRawData { get; set; }
    public int? TotalOutputTonnes
    {
        get
        {
            int total = SentToOtherSiteTonnes.ConvertToInt() + ContaminantTonnes.ConvertToInt() + ProcessLossTonnes.ConvertToInt();

            if (ReprocessedMaterialsRawData != null && ReprocessedMaterialsRawData.Count > 0)
            {
                total += ReprocessedMaterialsRawData
                    .Where(rm => !string.IsNullOrWhiteSpace(rm.MaterialOrProductName) && !string.IsNullOrWhiteSpace(rm.ReprocessedTonnes))
                    .Sum(rm => rm.ReprocessedTonnes.ConvertToInt());
            }
            return total;
        }
    }   
}