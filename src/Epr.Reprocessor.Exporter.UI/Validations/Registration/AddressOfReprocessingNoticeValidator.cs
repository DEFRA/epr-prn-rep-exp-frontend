using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using FluentValidation;

namespace Epr.Reprocessor.Exporter.UI.Validations.Registration;
public class AddressOfReprocessingNoticeValidator : AbstractValidator<AddressForNoticesViewModel>
{
    public AddressOfReprocessingNoticeValidator()
    {
        RuleFor(x => x.SelectedAddressOptions)
            .NotEmpty()
            .WithMessage(AddressForNotices.select_the_address_for_notifications);
    }
}
