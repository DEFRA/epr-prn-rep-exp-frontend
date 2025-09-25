namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation; 

[ExcludeFromCodeCoverage]
public class FileUploadViewModel
{
    public Guid ExternalId { get; set; } = Guid.Empty;

    public Guid FileId { get; set; } = Guid.Empty;

    public string FileName { get; set; } = string.Empty;
    
    public DateTime DateUploaded { get; set; } = DateTime.MinValue;

    public string UploadedBy { get; set; } = string.Empty;

    public string DownloadFileUrl { get; set; }

    public string DeleteFileUrl { get; set; }
}
