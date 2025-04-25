using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class SamplingAndInspectionPlanViewModel
    {
        public string MaterialName { get; set; } = string.Empty;

        public List<FileUploadViewModel> UploadedFiles { get; set; } = new List<FileUploadViewModel>();
    }
}
