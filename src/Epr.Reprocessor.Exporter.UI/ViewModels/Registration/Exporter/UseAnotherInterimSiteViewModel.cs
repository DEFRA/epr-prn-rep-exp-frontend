using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

public class UseAnotherInterimSiteViewModel
{
    [Required]
    public bool? AddInterimSiteAccepted { get; set; }

    public string CompanyName { get; set; }

    public string AddressLine { get; set; }
}

