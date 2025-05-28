using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;

[ExcludeFromCodeCoverage]
public class ApplicationSubmissionConfirmationViewModel
{
    public string ApplicationReferenceNumber { get; set; } = string.Empty;

    public UkNation SiteLocation { get; set; }
    
    public bool IsComplianceScheme { get; set; }
}
