namespace Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney
{
	public class OtherPermitsDto
    {
		public int Id { get; set; }

		public int RegistrationId { get; set; }

		public string WasteLicenseOrPermitNumber { get; set; }

		public string PpcNumber { get; set; }

		public List<string> WasteExemptionReference { get; set; } = new List<string>();
	}
}
