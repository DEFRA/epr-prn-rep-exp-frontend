using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Epr.Reprocessor.Exporter.UI.Enums
{
    /// <summary>
    /// Registration Status Enumeration based on https://eaflood.atlassian.net/wiki/spaces/MWR/pages/5539856513/View+registration+applications+and+status+labels
    /// consider moving this to the ui enums
    /// </summary>
    public enum RegistrationStatus
    {
        //[Display(Name = "In progress", ResourceType = typeof(RegistrationStatusResource))]
        InProgress,
        [Display(Name = "Completed")]
        Completed,
        [Display(Name = "Submitted")]
        Submitted,
        [Display(Name = "Regulator reviewing")]
        RegulatorReviewing,
        [Display(Name = "Queried")]
        Queried,
        [Display(Name = "Updated")]
        Updated,
        [Display(Name = "Refused")]
        Refused,
        [Display(Name = "Granted")]
        Granted,
        [Display(Name = "Renewal in progress")]
        RenewalInProgress,
        [Display(Name = "Renewal submitted")]
        RenewalSubmitted,
        [Display(Name = "Renewal queried")]
        RenewalQueried,
        [Display(Name = "Suspended")] 
        Suspended,
        [Display(Name = "Cancelled")] 
        Cancelled,
        [Display(Name = "Needs to be renewed")] 
        NeedsToBeRenewed,

    }
}
