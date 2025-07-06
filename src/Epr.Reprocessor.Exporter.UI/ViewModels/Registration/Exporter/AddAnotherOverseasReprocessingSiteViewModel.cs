using System.ComponentModel.DataAnnotations;


namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

public class AddAnotherOverseasReprocessingSiteViewModel
{
    [Required]
    public bool? AddOverseasSiteAccepted { get; set; }

}


