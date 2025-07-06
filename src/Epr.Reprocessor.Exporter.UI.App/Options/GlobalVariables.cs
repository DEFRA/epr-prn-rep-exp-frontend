namespace Epr.Reprocessor.Exporter.UI.App.Options;

[ExcludeFromCodeCoverage]
public class GlobalVariables
{
    public string BasePath { get; set; }

    public int FileUploadLimitInBytes { get; set; }

    public int AccreditationFileUploadLimitInBytes { get; set; }

    public bool UseLocalSession { get; set; }

    public string LogPrefix { get; set; }
}
