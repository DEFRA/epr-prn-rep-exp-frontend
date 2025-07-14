using Epr.Reprocessor.Exporter.UI.App.Helpers;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Exporter;
using Epr.Reprocessor.Exporter.UI.Validations.Shared;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;
using FluentValidation;

namespace Epr.Reprocessor.Exporter.UI.Validations.Registration.Exporter;

public class UseAnotherInterimSiteViewValidator : AbstractValidator<UseAnotherInterimSiteViewModel>
{
    public UseAnotherInterimSiteViewValidator()
    {
        RuleFor(x => x.AddInterimSiteAccepted)
           .NotNull()
           .WithMessage(UseAnotherInterimSite.UseAnotherInterimSiteErrorMessage);
    }
}

