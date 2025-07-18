﻿using Epr.Reprocessor.Exporter.UI.Resources.Views.ExporterJourney.OtherPermits;
using Epr.Reprocessor.Exporter.UI.Resources.Views.ExporterJourney.WasteCarrierBrokerDealerReference;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney
{
    public class OtherPermitsViewModel: ExporterViewModelBase
    {
        public Guid Id { get; set; }

        [MaxLength(16, ErrorMessageResourceName = "carrier_broker_dealer_registration_number_exceeds_length", ErrorMessageResourceType = typeof(WasteCarrierBrokerDealerReference))]
        public string? WasteCarrierBrokerDealerRegistration { get; set; }

        [MaxLength(20, ErrorMessageResourceName = "maximum_permit_ref_length", ErrorMessageResourceType = typeof(OtherPermits))]
		public string? WasteLicenseOrPermitNumber { get; set; }

        [MaxLength(20, ErrorMessageResourceName = "maximum_permit_ref_length", ErrorMessageResourceType = typeof(OtherPermits))]
		public string? PpcNumber { get; set; }

        [MaxLength(50, ErrorMessageResourceName = "maximum_permit_ref_length", ErrorMessageResourceType = typeof(OtherPermits))]
        public List<string>? WasteExemptionReference { get; set; } = new List<string> { string.Empty };

    }
}
