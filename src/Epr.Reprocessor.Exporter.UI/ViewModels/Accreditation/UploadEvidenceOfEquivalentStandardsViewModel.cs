namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;

[ExcludeFromCodeCoverage]
public class UploadEvidenceOfEquivalentStandardsViewModel
{
    public bool IsSiteOutsideEU_OECD { get; set; } = true;

    public string MaterialName { get; set; } = string.Empty;

    public bool IsMetallicMaterial => MaterialName.ToLower() is "aluminium" or "steel";

    public List<OverseasReprocessingSite> OverseasSites { get; set; }
}
