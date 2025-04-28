using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels
{
    public class AddressOfReprocessingSiteViewModel
    {
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string TownCity { get; set; }

        public string County { get; set; }

        public string Postcode { get; set; }
    }
}