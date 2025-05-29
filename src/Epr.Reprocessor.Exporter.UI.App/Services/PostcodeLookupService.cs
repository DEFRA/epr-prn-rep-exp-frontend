using Epr.Reprocessor.Exporter.UI.App.Clients.Interfaces;
using Epr.Reprocessor.Exporter.UI.App.DTOs.AddressLookup;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

/// <summary>
/// Service that handles address lookup operations based on a postcode.
/// </summary>
public class PostcodeLookupService : IPostcodeLookupService
{
    private readonly IPostcodeLookupApiClient _postcodeLookupApiClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="PostcodeLookupService"/> class.
    /// </summary>
    /// <param name="postcodeLookupApiClient">An API client used to retrieve address information based on a postcode.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="postcodeLookupApiClient"/> is null.</exception>
    public PostcodeLookupService(IPostcodeLookupApiClient postcodeLookupApiClient)
    {
        _postcodeLookupApiClient = postcodeLookupApiClient;
    }

    /// <summary>
    /// Retrieves a list of addresses corresponding to the given postcode.
    /// </summary>
    /// <param name="postcode">The postcode to use for the lookup.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AddressList"/> of matching addresses.</returns>
    public async Task<AddressList> GetAddressListByPostcodeAsync(string postcode)
    {
        return await _postcodeLookupApiClient.GetAddressListByPostcodeAsync(postcode);
    }
}
