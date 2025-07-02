using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;
using FluentValidation;


namespace Epr.Reprocessor.Exporter.UI.Validations.Registration.Exporter;

public class InterimSitesQuestionOneViewModelValidator : AbstractValidator<InterimSitesQuestionOneViewModel>
{

    public InterimSitesQuestionOneViewModelValidator()
    {
        RuleFor(x => x.HasInterimSites)
            .NotNull()
            .NotEmpty()
            .WithMessage(InterimSitesQuestionOne.Please_select_an_option);
    }
}

