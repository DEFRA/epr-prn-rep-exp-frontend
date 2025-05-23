﻿using Epr.Reprocessor.Exporter.UI.App.Helpers;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Accreditation;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration;
using FluentValidation;

namespace Epr.Reprocessor.Exporter.UI.Validations.Accreditation
{
    public class SelectAuthorityValidator : AbstractValidator<SelectAuthorityViewModel>
    {
        public SelectAuthorityValidator()
        {
            When(x => x.Subject == "PRN", () =>
            {
                // Minimum Length of trailing digits
                RuleFor(x => x.SelectedAuthoritiesCount)
                    .GreaterThan(0)
                    .WithMessage(SelectAuthority.error_message_prns);
            });

            When(x => x.Subject == "PERN", () =>
            {
                // Minimum Length of trailing digits
                RuleFor(x => x.SelectedAuthoritiesCount)
                    .GreaterThan(0)
                    .WithMessage(SelectAuthority.error_message_perns);
            });
        }
    }
}
