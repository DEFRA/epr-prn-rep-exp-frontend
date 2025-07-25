﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Shared;


namespace Epr.Reprocessor.Exporter.UI.Enums
{
    /// <summary>
    /// Registration Status Enumeration based on https://eaflood.atlassian.net/wiki/spaces/MWR/pages/5539856513/View+registration+applications+and+status+labels
    /// consider moving this to the ui enums
    /// </summary>
    public enum RegistrationStatus
    {
        [Display(Name = "InProgress", ResourceType = typeof(RegistrationStatusResource))]
        InProgress,
        [Display(Name = "Completed", ResourceType = typeof(RegistrationStatusResource))]
        Completed,
        [Display(Name = "Submitted", ResourceType = typeof(RegistrationStatusResource))]
        Submitted,
        [Display(Name = "RegulatorReviewing", ResourceType = typeof(RegistrationStatusResource))]
        RegulatorReviewing,
        [Display(Name = "Queried", ResourceType = typeof(RegistrationStatusResource))]
        Queried,
        [Display(Name = "Updated", ResourceType = typeof(RegistrationStatusResource))]
        Updated,
        [Display(Name = "Refused", ResourceType = typeof(RegistrationStatusResource))]
        Refused,
        [Display(Name = "Granted", ResourceType = typeof(RegistrationStatusResource))]
        Granted,
        [Display(Name = "RenewalInProgress", ResourceType = typeof(RegistrationStatusResource))]
        RenewalInProgress,
        [Display(Name = "RenewalSubmitted", ResourceType = typeof(RegistrationStatusResource))]
        RenewalSubmitted,
        [Display(Name = "RenewalQueried", ResourceType = typeof(RegistrationStatusResource))]
        RenewalQueried,
        [Display(Name = "Suspended", ResourceType = typeof(RegistrationStatusResource))]
        Suspended,
        [Display(Name = "Cancelled", ResourceType = typeof(RegistrationStatusResource))]
        Cancelled,
        [Display(Name = "NeedsToBeRenewed", ResourceType = typeof(RegistrationStatusResource))]
        NeedsToBeRenewed,

    }
}
