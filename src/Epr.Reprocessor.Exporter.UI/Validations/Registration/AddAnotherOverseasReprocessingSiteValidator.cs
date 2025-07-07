using Epr.Reprocessor.Exporter.UI.App.Helpers;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Exporter;
using Epr.Reprocessor.Exporter.UI.Validations.Shared;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;
using FluentValidation;


namespace Epr.Reprocessor.Exporter.UI.Validations.Registration;

public class AddAnotherOverseasReprocessingSiteValidator : AbstractValidator<AddAnotherOverseasReprocessingSiteViewModel>
{

    public AddAnotherOverseasReprocessingSiteValidator()
    {
        RuleFor(x => x.AddOverseasSiteAccepted)
            .NotNull()            
            .WithMessage(AddAnotherOverseasReprocessingSite.AddOverseasProcessingSiteErrorMessage);
    }
}

