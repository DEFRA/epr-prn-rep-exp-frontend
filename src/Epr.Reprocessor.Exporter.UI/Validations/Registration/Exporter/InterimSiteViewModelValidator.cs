using FluentValidation;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.Validations.Registration;

public class InterimSiteViewModelValidator : AbstractValidator<InterimSiteViewModel>
{
    public InterimSiteViewModelValidator()
    {
        RuleFor(x => x.Country)
            .NotEmpty().WithMessage(InterimSiteDetails.CountryRequired);

        RuleFor(x => x.OrganisationName)
            .NotEmpty().WithMessage(InterimSiteDetails.OrganisationNameRequired);

        RuleFor(x => x.AddressLine1)
            .NotEmpty().WithMessage(InterimSiteDetails.AddressLine1Required)
            .MaximumLength(100).WithMessage(InterimSiteDetails.AddressLine1MaxLength);

        RuleFor(x => x.AddressLine2)
            .MaximumLength(100).WithMessage(InterimSiteDetails.AddressLine2MaxLength);

        RuleFor(x => x.CityorTown)
            .NotEmpty().WithMessage(InterimSiteDetails.CityorTownRequired)
            .MaximumLength(70).WithMessage(InterimSiteDetails.CityorTownMaxLength);

        RuleFor(x => x.StateProvince)
            .MaximumLength(70).WithMessage(InterimSiteDetails.StateProvinceMaxLength);

        RuleFor(x => x.Postcode)
            .MaximumLength(20).WithMessage(InterimSiteDetails.PostcodeMaxLength);

        RuleFor(x => x.ContactFullName)
            .NotEmpty().WithMessage(InterimSiteDetails.ContactFullNameRequired)
            .MaximumLength(100).WithMessage(InterimSiteDetails.ContactFullNameMaxLength);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(InterimSiteDetails.EmailRequired)
            .EmailAddress().WithMessage(InterimSiteDetails.EmailInvalid)
            .MaximumLength(100).WithMessage(InterimSiteDetails.EmailMaxLength);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage(InterimSiteDetails.PhoneNumberRequired)
            .MaximumLength(25).WithMessage(InterimSiteDetails.PhoneNumberMaxLength);
    }
}
