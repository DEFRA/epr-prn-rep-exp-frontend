using Epr.Reprocessor.Exporter.UI.Attributes.Validations;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney
{
    [ExcludeFromCodeCoverage]
    public class AddressSearchViewModel
    {
        public AddressSearchViewModel()
        {

        }

        public AddressSearchViewModel(string pastcode)
        {
            Postcode = Postcode;
        }
        public string? Postcode { get; set; }
    }
}
