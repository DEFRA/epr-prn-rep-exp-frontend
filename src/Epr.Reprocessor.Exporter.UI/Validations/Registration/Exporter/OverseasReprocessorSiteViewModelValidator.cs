using FluentValidation;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.Validations.Registration
{
    public class OverseasReprocessorSiteViewModelValidator : AbstractValidator<OverseasReprocessorSiteViewModel>
    {
        public OverseasReprocessorSiteViewModelValidator()
        {
            RuleFor(x => x.Country)
                .NotEmpty().WithMessage(OverseasSiteDetails.CountryRequired); // Add to .resx: "Select the country the site is in"

            RuleFor(x => x.OrganisationName)
                .NotEmpty().WithMessage(OverseasSiteDetails.OrganisationNameRequired); // Add to .resx: "Enter the organisation’s name"

            RuleFor(x => x.AddressLine1)
                .NotEmpty().WithMessage(OverseasSiteDetails.AddressLine1Required) // Add to .resx: "Enter address line 1, typically the building and street"
                .MaximumLength(100).WithMessage(OverseasSiteDetails.AddressLine1MaxLength); // Add to .resx: "Address line 1 must be 100 characters or fewer"

            RuleFor(x => x.AddressLine2)
                .MaximumLength(100).WithMessage(OverseasSiteDetails.AddressLine2MaxLength); // Add to .resx: "Address line 2 must be 100 characters or fewer"

            RuleFor(x => x.CityorTown)
                .NotEmpty().WithMessage(OverseasSiteDetails.CityorTownRequired) // Add to .resx: "Enter a city or town"
                .MaximumLength(70).WithMessage(OverseasSiteDetails.CityorTownMaxLength); // Add to .resx: "City or town must be 70 characters or fewer"

            RuleFor(x => x.StateProvince)
                .MaximumLength(70).WithMessage(OverseasSiteDetails.StateProvinceMaxLength); // Add to .resx: "State, province or region must be 70 characters or fewer"

            RuleFor(x => x.Postcode)
                .MaximumLength(20).WithMessage(OverseasSiteDetails.PostcodeMaxLength); // Add to .resx: "Postcode must be 20 characters or fewer"

            RuleFor(x => x.SiteCoordinates)
                .NotEmpty().WithMessage(OverseasSiteDetails.SiteCoordinatesRequired) // Add to .resx: "Enter the latitude and longitude coordinates for the site’s main entrance"
                .MaximumLength(100).WithMessage(OverseasSiteDetails.SiteCoordinatesMaxLength); // Add to .resx: "Coordinates must be 100 characters or fewer"

            RuleFor(x => x.ContactFullName)
                .NotEmpty().WithMessage(OverseasSiteDetails.ContactFullNameRequired) // Add to .resx: "Enter the name of the person the regulator can contact"
                .MaximumLength(100).WithMessage(OverseasSiteDetails.ContactFullNameMaxLength); // Add to .resx: "Full name must be 100 characters or fewer"

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(OverseasSiteDetails.EmailRequired) // Add to .resx: "Enter the email of the person the regulator can contact"
                .EmailAddress().WithMessage(OverseasSiteDetails.EmailInvalid) // Add to .resx: "Enter an email address in the correct format, like name@example.com"
                .MaximumLength(100).WithMessage(OverseasSiteDetails.EmailMaxLength); // Add to .resx: "Email must be 100 characters or fewer"

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage(OverseasSiteDetails.PhoneNumberRequired) // Add to .resx: "Enter the phone number of the person the regulator can contact"
                .MaximumLength(25).WithMessage(OverseasSiteDetails.PhoneNumberMaxLength); // Add to .resx: "Phone number must be 25 characters or fewer"
        }
    }
}
