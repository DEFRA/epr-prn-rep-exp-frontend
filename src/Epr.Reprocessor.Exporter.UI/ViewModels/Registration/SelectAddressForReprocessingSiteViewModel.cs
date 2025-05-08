using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

[ExcludeFromCodeCoverage]
public class SelectAddressForReprocessingSiteViewModel
{
    public string Postcode { get; set; }
    public int? SelectedIndex { get; set; }
    public List<AddressViewModel> Addresses { get; set; } = new List<AddressViewModel>();
}
