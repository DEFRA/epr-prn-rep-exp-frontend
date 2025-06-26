namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class SamplingAndInspectionPlanViewModel
    {
        public Guid AccreditationId { get; set; }

        public string MaterialName { get; set; } = string.Empty;

        public IFormFile File { get; set; }

        public List<FileUploadViewModel> UploadedFiles { get; set; } = new List<FileUploadViewModel>();
    }
}
