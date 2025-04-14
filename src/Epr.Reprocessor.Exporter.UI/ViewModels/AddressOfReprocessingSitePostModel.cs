using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels
{
    public class AddressOfReprocessingSitePostModel
    {
        [Required(ErrorMessageResourceName = "SelectAnOptionErrorMessage", ErrorMessageResourceType = typeof(AddressOfReprocessingSite))]
        public bool? IsSameAddress { get; set; } = null;
    }
}
