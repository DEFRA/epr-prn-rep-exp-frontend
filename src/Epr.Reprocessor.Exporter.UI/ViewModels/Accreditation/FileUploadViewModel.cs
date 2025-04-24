using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class FileUploadViewModel
    {
        public string FileName { get; set; } = string.Empty;
        
        public DateTime DateUploaded { get; set; }

        public string UploadedBy { get; set; } = string.Empty;
    }
}
