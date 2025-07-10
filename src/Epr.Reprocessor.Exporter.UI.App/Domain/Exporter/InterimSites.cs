namespace Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;

public class InterimSites
{
    public bool? HasInterimSites { get; set; }
    public List<OverseasMaterialReprocessingSite> OverseasMaterialReprocessingSites { get; set; } = new();
}