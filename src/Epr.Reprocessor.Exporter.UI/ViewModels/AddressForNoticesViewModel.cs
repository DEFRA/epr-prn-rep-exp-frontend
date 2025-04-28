using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.App.Enums; 

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

    public class AddressForNoticesViewModel
{
    [Required(ErrorMessage = "Select an address for service of notices.")]

    public AddressOptions SelectedAddressOptions { get; set; } 
    }

