using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

public class AddressLookUpService : IAddressLookUpService
{
    private IEprFacadeServiceApiClient _client;
    private ILogger<AddressLookUpService> _logger;


    public AddressLookUpService(IEprFacadeServiceApiClient client, ILogger<AddressLookUpService> logger)
    {
        _client = client;
        _logger = logger;        
    }

    public async Task<IEnumerable<AddressDto>> GetListOfAddress(string postcode)
    {
        try
        {
            var result = await _client.SendPostRequest(EprPrnFacadePaths.AddressLookUpByPostcode, postcode);
            var content = await result.Content.ReadAsStringAsync();

            result.EnsureSuccessStatusCode();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to look up addresses for postcode {postcode}", postcode);
            throw;
        }
    }
}
