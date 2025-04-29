using Epr.Reprocessor.Exporter.UI.Enums;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

public class AddressOfReprocessingSiteViewModel
{
    public ReprocessingSiteAddressOptions? SelectedOption { get; set; }

    public AddressViewModel? BusinessAddress { get; set; }
}