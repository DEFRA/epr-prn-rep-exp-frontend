using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.Enums;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class AddressForNoticesViewModel
{
    [Required(ErrorMessage = "Select an address for service of notices.")]

    public AddressOptions SelectedAddressOptions { get; set; }
    
    public AddressViewModel? AddressToShow { get; set; }
}

