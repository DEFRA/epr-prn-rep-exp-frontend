namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class EvidenceOfEquivalentStandardsCheckYourAnswerViewModel
    {
        public string? Action { get; set; }
        public string? SiteName { get; set; }
        public string? SiteAddressLine1 { get; set; }
        public string? SiteAddressLine2 { get; set; }
        public string? SiteAddressLine3 { get; set; }
        public string? ValidFrom { get; set; }
        public string? ExpireDate { get; set; }
        public string? UploadedFile { get; set; }
        public bool MoreEvidenceUpload { get; set; }
    }
}
