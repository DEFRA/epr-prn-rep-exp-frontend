using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

[ExcludeFromCodeCoverage]
public class SelectAddressForReprocessingSiteViewModel
{
    public SelectAddressForReprocessingSiteViewModel()
    {

    }

    public SelectAddressForReprocessingSiteViewModel(Domain.LookupAddress manualAddress)
    {
        Postcode = manualAddress.Postcode;
        SelectedIndex = manualAddress.SelectedAddressIndex;
        Addresses = manualAddress.AddressesForPostcode.Select(x => new AddressViewModel
        {
            AddressLine1 = x.AddressLine1,
            AddressLine2 = x.AddressLine2,
            County = x.County,
            Postcode = x.Postcode,
            TownOrCity = x.Town,
        }).ToList();
    }

    public string Postcode { get; set; }
    public int? SelectedIndex { get; set; }
    public List<AddressViewModel> Addresses { get; set; } = new List<AddressViewModel>();
}
