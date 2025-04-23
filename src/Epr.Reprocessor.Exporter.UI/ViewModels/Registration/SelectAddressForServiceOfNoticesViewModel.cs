namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

public class SelectAddressForServiceOfNoticesViewModel
{
    public string Postcode { get; set; }
    public int? SelectedIndex { get; set; }
    public List<AddressViewModel> Addresses { get; set; } = new List<AddressViewModel>();
}
