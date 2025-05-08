namespace Epr.Reprocessor.Exporter.UI.App.DTOs;

    public class AddressDto
    {
        public string AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; }

        public string TownOrCity { get; set; }

        public string? County { get; set; }

        public string Postcode { get; set; }

        public string FormattedAddress => string.Join(", ", new[] { AddressLine1, AddressLine2, TownOrCity, County, Postcode }
            .Where(addressPart => !string.IsNullOrWhiteSpace(addressPart)));
    }

