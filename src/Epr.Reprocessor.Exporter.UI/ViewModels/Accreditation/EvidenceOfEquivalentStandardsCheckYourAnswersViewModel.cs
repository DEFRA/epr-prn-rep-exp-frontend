namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;

[ExcludeFromCodeCoverage]
public class EvidenceOfEquivalentStandardsCheckYourAnswersViewModel
{
    public OverseasReprocessingSite OverseasSite { get; set; }

    public bool SiteFulfillsAllConditions { get; set; }

    public string UploadedFileName { get; set; }

    public DateTime StartDateOfEvidence { get; set; }

    public DateTime ExpiryDateOfEvidence { get; set; }

    public bool MoreEvidenceToUpload { get; set; }
}
