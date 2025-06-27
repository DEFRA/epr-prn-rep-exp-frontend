using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter
{
    public class OverseasReprocessorSiteViewModel
    {
        [BindNever, ValidateNever]
        public IEnumerable<string> Countries { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string OrganisationName { get; set; }

        [Required]
        [MaxLength(100)]
        public string AddressLine1 { get; set; }
        [MaxLength(100)]
        public string AddressLine2 { get; set; }

        [Required]
        [MaxLength(70)]
        public string CityorTown { get; set; }
        [MaxLength(70)]
        public string StateProvince { get; set; }
        [MaxLength(20)]
        public string Postcode { get; set; }
        [MaxLength(100)]
        public string SiteCoordinates { get; set; }

        [Required]
        public string ContactFullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}