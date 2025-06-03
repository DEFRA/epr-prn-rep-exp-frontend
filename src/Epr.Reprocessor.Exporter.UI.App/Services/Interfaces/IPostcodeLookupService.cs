using Epr.Reprocessor.Exporter.UI.App.DTOs.AddressLookup;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

public interface IPostcodeLookupService
{
    Task<AddressList> GetAddressListByPostcodeAsync(string postcode);
}
