using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Reprocessor;

public class ReprocessedMaterialRawDataModel
{
    public string? MaterialOrProductName { get; set; }
    public decimal? ReprocessedTonnes { get; set; }
}