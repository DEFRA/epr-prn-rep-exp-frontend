using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.Enums;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

[ExcludeFromCodeCoverage]
public class AddressOfReprocessingSiteViewModel
{
    public AddressOptions? SelectedOption { get; set; }

    public AddressViewModel? BusinessAddress { get; set; }

    public AddressViewModel? RegisteredAddress { get; set; }
}