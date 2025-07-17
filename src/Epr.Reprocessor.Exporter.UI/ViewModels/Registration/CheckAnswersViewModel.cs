namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

[ExcludeFromCodeCoverage]
public class CheckAnswersViewModel 
{
    public CheckAnswersViewModel()
    {
    }

    public CheckAnswersViewModel(ReprocessingSite reprocessingSite)
    {
        ReprocessingSite = reprocessingSite;
        // Site Location
        SiteLocation = reprocessingSite.Nation;

        // Site Grid Reference
        SiteGridReference = reprocessingSite.SiteGridReference;

        // Reprocessing Site Address
        ReprocessingSiteAddress = new AddressViewModel(reprocessingSite.Address);

        // Service Of Notices Address
        ServiceOfNoticesAddress = new AddressViewModel(reprocessingSite.ServiceOfNotice.Address);
    }

    public UkNation? SiteLocation { get; set; }
    public AddressViewModel ReprocessingSiteAddress { get; set; }
    public string SiteGridReference { get; set; }
    public AddressViewModel ServiceOfNoticesAddress { get; set; }
    public ReprocessingSite ReprocessingSite { get; set; }

    /// <summary>
    /// Calculates the originating page for the check your answers page based on the type of address provided for service of notices.
    /// </summary>
    /// <returns>The name of the originating page.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Throws when the type of address is not in range.</exception>
    public string CalculateOriginationPage()
    {
        var isLookup = ReprocessingSite.ServiceOfNotice?.LookupAddress?.SelectedAddress is not null;
        var typeOfAddress = ReprocessingSite.ServiceOfNotice?.TypeOfAddress;

        return typeOfAddress switch
        {
            AddressOptions.RegisteredAddress or AddressOptions.BusinessAddress or AddressOptions.SiteAddress => PagePaths.AddressForNotices,
            AddressOptions.DifferentAddress => isLookup ? PagePaths.ConfirmNoticesAddress : PagePaths.ManualAddressForServiceOfNotices,
            _ => throw new ArgumentOutOfRangeException(nameof(typeOfAddress), typeOfAddress, null)
        };
    }
}