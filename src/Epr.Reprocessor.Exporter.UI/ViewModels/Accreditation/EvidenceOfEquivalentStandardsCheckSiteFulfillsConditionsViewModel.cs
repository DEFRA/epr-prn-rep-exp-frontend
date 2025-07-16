using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;

[ExcludeFromCodeCoverage]
public class EvidenceOfEquivalentStandardsCheckSiteFulfillsConditionsViewModel
{
    public OverseasReprocessingSite OverseasSite { get; set; }

    public bool SiteFulfillsAllConditions { get; set; }

    public Guid AccreditationId { get; set; }

    public string? Action { get; set; }

    public string? SelectedOption { get; set; }
}
