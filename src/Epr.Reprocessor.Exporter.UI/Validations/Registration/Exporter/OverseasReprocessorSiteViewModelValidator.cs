using FluentValidation;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.Validations.Registration;

public class OverseasReprocessorSiteViewModelValidator : AbstractValidator<OverseasReprocessorSiteViewModel>
{
    public OverseasReprocessorSiteViewModelValidator()
    {
        RuleFor(x => x.Country)
            .NotEmpty().WithMessage(OverseasSiteDetails.CountryRequired);

        RuleFor(x => x.OrganisationName)
            .NotEmpty().WithMessage(OverseasSiteDetails.OrganisationNameRequired);

        RuleFor(x => x.AddressLine1)
            .NotEmpty().WithMessage(OverseasSiteDetails.AddressLine1Required)
            .MaximumLength(100).WithMessage(OverseasSiteDetails.AddressLine1MaxLength);

        RuleFor(x => x.AddressLine2)
            .MaximumLength(100).WithMessage(OverseasSiteDetails.AddressLine2MaxLength);

        RuleFor(x => x.CityorTown)
            .NotEmpty().WithMessage(OverseasSiteDetails.CityorTownRequired)
            .MaximumLength(70).WithMessage(OverseasSiteDetails.CityorTownMaxLength);

        RuleFor(x => x.StateProvince)
            .MaximumLength(70).WithMessage(OverseasSiteDetails.StateProvinceMaxLength);

        RuleFor(x => x.Postcode)
            .MaximumLength(20).WithMessage(OverseasSiteDetails.PostcodeMaxLength);

        RuleFor(x => x.SiteCoordinates)
            .NotEmpty().WithMessage(OverseasSiteDetails.SiteCoordinatesRequired)
            .MaximumLength(100).WithMessage(OverseasSiteDetails.SiteCoordinatesMaxLength);

        RuleFor(x => x.ContactFullName)
            .NotEmpty().WithMessage(OverseasSiteDetails.ContactFullNameRequired)
            .MaximumLength(100).WithMessage(OverseasSiteDetails.ContactFullNameMaxLength);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(OverseasSiteDetails.EmailRequired)
            .EmailAddress().WithMessage(OverseasSiteDetails.EmailInvalid)
            .MaximumLength(100).WithMessage(OverseasSiteDetails.EmailMaxLength);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage(OverseasSiteDetails.PhoneNumberRequired)
            .MaximumLength(25).WithMessage(OverseasSiteDetails.PhoneNumberMaxLength);
    }
}
