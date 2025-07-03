namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class SamplingAndInspectionPlanViewModel
    {
        public Guid AccreditationId { get; set; }

        public int ApplicationTypeId { get; set; }

        public IFormFile? File { get; set; }

        public string? Action { get; set; }

        public NotificationBannerModel? SuccessBanner { get; set; }

        public List<FileUploadViewModel> UploadedFiles { get; set; } = new List<FileUploadViewModel>();
    }
}
