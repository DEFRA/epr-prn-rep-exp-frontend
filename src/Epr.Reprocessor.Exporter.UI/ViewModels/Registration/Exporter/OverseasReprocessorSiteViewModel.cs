using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter
{
    public class OverseasReprocessorSiteViewModel
    {
        public IEnumerable<string> Countries { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string OrganisationName { get; set; }

        [Required]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        [Required]
        public string TownOrCity { get; set; }

        public string StateProvinceRegion { get; set; }

        public string Postcode { get; set; }

        public string Coordinates { get; set; }

        [Required]
        public string ContactFullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}