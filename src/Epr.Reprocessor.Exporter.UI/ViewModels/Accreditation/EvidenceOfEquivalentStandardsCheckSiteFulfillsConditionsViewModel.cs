using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;

[ExcludeFromCodeCoverage]
public class EvidenceOfEquivalentStandardsCheckSiteFulfillsConditionsViewModel
{
    public OverseasReprocessingSite OverseasSite { get; set; }

    public bool SiteFulfillsAllConditions { get; set; } = false;

    public Guid AccreditationId { get; set; } = Guid.Empty;

    public string? Action { get; set; }

    public string? SelectedOption { get; set; }
}
