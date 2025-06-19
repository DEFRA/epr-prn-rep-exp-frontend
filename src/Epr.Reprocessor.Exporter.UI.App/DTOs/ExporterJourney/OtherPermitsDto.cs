namespace Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney
{
    [ExcludeFromCodeCoverage]
    public class OtherPermitsDto
    {
		public Guid Id { get; set; }

		public Guid RegistrationId { get; set; }

		public string WasteLicenseOrPermitNumber { get; set; }

		public string PpcNumber { get; set; }

		public List<string> WasteExemptionReference { get; set; } = new List<string>();
	}
}
