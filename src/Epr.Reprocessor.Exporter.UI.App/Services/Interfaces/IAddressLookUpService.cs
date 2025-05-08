using Epr.Reprocessor.Exporter.UI.App.DTOs;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

public interface IAddressLookUpService
{
    Task<IEnumerable<AddressDto>> GetListOfAddress(string postcode);
}

