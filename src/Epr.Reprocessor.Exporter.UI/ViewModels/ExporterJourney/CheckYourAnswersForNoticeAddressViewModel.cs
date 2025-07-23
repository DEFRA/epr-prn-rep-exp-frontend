namespace Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney
{
    public class CheckYourAnswersForNoticeAddressViewModel
    {
        public string AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; } = string.Empty;

        public string TownCity { get; set; }

        public string? County { get; set; } = string.Empty;

        public string? Country { get; set; } = string.Empty;

        public string PostCode { get; set; }

        public int? NationId { get; set; } = 0;

        public string GridReference { get; set; }

        public string FormattedAddress => string.Join(", ", new[] { AddressLine1, AddressLine2, TownCity, County, PostCode }
                                          .Where(addressPart => !string.IsNullOrWhiteSpace(addressPart)));
    }
}
