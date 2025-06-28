using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter
{
    public class OverseasReprocessorSiteViewModel
    {
        [BindNever, ValidateNever]
        public bool IsFirstSite { get; set; }
        [BindNever, ValidateNever]
        public IEnumerable<string> Countries { get; set; }

        [Required(ErrorMessage = "Select the country the site is in")]
        public string Country { get; set; }

        [Required(ErrorMessage="Enter the organisation’s name")]
        public string OrganisationName { get; set; }

        [Required(ErrorMessage = "Enter address line 1, typically the building and street")]
        [MaxLength(100)]
        public string AddressLine1 { get; set; }
        [MaxLength(100)]
        public string? AddressLine2 { get; set; }

        [Required(ErrorMessage = "Enter a city or town")]
        [MaxLength(70)]
        public string CityorTown { get; set; }
        [MaxLength(70)]
        public string? StateProvince { get; set; }
        [MaxLength(20)]
        public string? Postcode { get; set; }
        [Required(ErrorMessage = "Enter the latitude and longitude coordinates for the site’s main entrance")]
        [MaxLength(100)]
        public string SiteCoordinates { get; set; }
        [Required(ErrorMessage = "Enter the name of the person the regulator can contact")]
        [MaxLength(100)]
        public string ContactFullName { get; set; }
        [Required(ErrorMessage = "Enter the email of the person the regulator can contact")]
        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [MaxLength(100)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Enter the phone number of the person the regulator can contact")]
        [MaxLength(25)]
        public string PhoneNumber { get; set; }
    }
}