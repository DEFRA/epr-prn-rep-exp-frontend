using Epr.Reprocessor.Exporter.UI.Resources.Views.ExporterJourney.OtherPermits;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney
{
    public class OtherPermitsViewModel
    {
        public Guid Id { get; set; }
        public Guid RegistrationId { get; set; }

		[MaxLength(20, ErrorMessageResourceName = "maximum_permit_ref_length", ErrorMessageResourceType = typeof(OtherPermits))]
		public string WasteLicenseOrPermitNumber { get; set; }

		[MaxLength(20, ErrorMessageResourceName = "maximum_permit_ref_length", ErrorMessageResourceType = typeof(OtherPermits))]
		public string PpcNumber { get; set; }

		[MaxLength(50, ErrorMessageResourceName = "maximum_permit_ref_length", ErrorMessageResourceType = typeof(OtherPermits))]
		public List<string> WasteExemptionReference { get; set; } = new List<string>();
    }
}
