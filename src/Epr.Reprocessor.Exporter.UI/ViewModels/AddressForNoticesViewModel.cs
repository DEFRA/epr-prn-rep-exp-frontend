using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.Enums;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

public class AddressForNoticesViewModel
{
    [Required(ErrorMessage = "Select an address for service of notices.")]

    public AddressOptions SelectedAddressOptions { get; set; }
    
    public AddressViewModel? AddressToShow { get; set; }
}

