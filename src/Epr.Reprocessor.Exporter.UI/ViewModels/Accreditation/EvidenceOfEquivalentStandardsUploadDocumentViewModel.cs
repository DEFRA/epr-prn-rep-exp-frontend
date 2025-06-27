namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class EvidenceOfEquivalentStandardsUploadDocumentViewModel
    {
        public string? Action { get; set; }
        public string? SiteName { get; set; }
        public string? SiteAddressLine1 { get; set; }
        public string? SiteAddressLine2 { get; set; }
        public string? SiteAddressLine3 { get; set; }
        public string? ValidFromDay { get; set; }
        public string? ValidFromMonth { get; set; }
        public string? ValidFromYear { get; set; }
        public string? ExpireDateDay { get; set; }
        public string? ExpireDateMonth { get; set; }
        public string? ExpireDateYear { get; set; }
        public FileUploadViewModel? UploadedFile { get; set; }
    }
}
