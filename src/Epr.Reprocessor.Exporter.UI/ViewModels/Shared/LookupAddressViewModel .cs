namespace Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

[ExcludeFromCodeCoverage]
public class LookupAddressViewModel
{
    public LookupAddressViewModel()
    {

    }

    public LookupAddressViewModel(LookupAddress manualAddress)
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
