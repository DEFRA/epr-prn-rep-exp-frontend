using System;
using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Shared;

namespace Epr.Reprocessor.Exporter.UI.App.Enums.Accreditation
{
    /// <summary>
    /// Accreditation Status Enumeration
    /// </summary>
    public enum AccreditationStatus
    {
        [Display(Name = "Not accredited", ResourceType = typeof(AccreditationStatusResource))]
        NotAccredited = 0,

        [Display(Name = "In Progress", ResourceType = typeof(AccreditationStatusResource))]
        Started = 1,

        [Display(Name = "Submitted", ResourceType = typeof(AccreditationStatusResource))]
        Submitted = 2,

        [Display(Name = "Accepted", ResourceType = typeof(AccreditationStatusResource))]
        Accepted = 3,

        [Display(Name = "Queried", ResourceType = typeof(AccreditationStatusResource))]
        Queried = 4,

        [Display(Name = "Updated", ResourceType = typeof(AccreditationStatusResource))]
        Updated = 5,

        [Display(Name = "Granted", ResourceType = typeof(AccreditationStatusResource))]
        Granted = 6,

        [Display(Name = "Refused", ResourceType = typeof(AccreditationStatusResource))]
        Refused = 7,

        [Display(Name = "Withdrawn", ResourceType = typeof(AccreditationStatusResource))]
        Withdrawn = 8,

        [Display(Name = "Suspended", ResourceType = typeof(AccreditationStatusResource))]
        Suspended = 9,

        [Display(Name = "Cancelled", ResourceType = typeof(AccreditationStatusResource))]
        Cancelled = 10
    }
}
