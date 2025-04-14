using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class SelectAddressOfReprocessingSiteViewModel
{
    public string Postcode { get; set; }

    public List<SelectAddressOfReprocessingSiteAddressViewModel> Addresses { get; set; }
}
