using Epr.Reprocessor.Exporter.UI.App.DTOs.AddressLookup;

namespace Epr.Reprocessor.Exporter.UI.App.Clients.Interfaces;

public interface IPostcodeLookupApiClient
{
    Task<AddressList?> GetAddressListByPostcodeAsync(string postcode);
}
