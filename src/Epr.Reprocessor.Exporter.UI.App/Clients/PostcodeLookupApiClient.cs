using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Epr.Reprocessor.Exporter.UI.App.Clients.Interfaces;
using Epr.Reprocessor.Exporter.UI.App.DTOs.AddressLookup;
using Epr.Reprocessor.Exporter.UI.App.Extensions;
using Epr.Reprocessor.Exporter.UI.App.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

/// <summary>
/// API client responsible for retrieving address lists based on a postcode from the postcode lookup service.
/// </summary>
[ExcludeFromCodeCoverage]
public class PostcodeLookupApiClient : IPostcodeLookupApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly string[] _scopes;
    private readonly string _baseAddress;
    private readonly ILogger<PostcodeLookupApiClient> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PostcodeLookupApiClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to make API requests.</param>
    /// <param name="tokenAcquisition">The token acquisition service for authenticating API requests.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="options">Options containing API configuration values.</param>
    public PostcodeLookupApiClient(
        HttpClient httpClient,
        ITokenAcquisition tokenAcquisition,
        ILogger<PostcodeLookupApiClient> logger,
        IOptions<AccountsFacadeApiOptions> options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(tokenAcquisition);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(options);

        _httpClient = httpClient;
        _tokenAcquisition = tokenAcquisition;
        _logger = logger;
        _scopes = [options.Value.DownstreamScope];
        _baseAddress = options.Value.BaseEndpoint;
    }

    /// <summary>
    /// Retrieves a list of addresses that match the given postcode.
    /// </summary>
    /// <param name="postcode">The postcode to look up.</param>
    /// <returns>An <see cref="AddressList"/> or <c>null</c> if no content is returned from the API.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell","S1144:Unused private type", Justification ="used in debug when the post code search API down")]
    public async Task<AddressList?> GetAddressListByPostcodeAsync(string postcode)
    {
        try
        {
            await PrepareAuthenticatedClientAsync();

            var response = await _httpClient.GetAsync($"/api/address-lookup?postcode={postcode}");

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var addressResponse = await response.Content.ReadFromJsonAsync<AddressLookupResponse>();

            return new AddressList(addressResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Postcode lookup failed for postcode: {Postcode}", postcode);
            throw;
        }
    }

    #region Private Methods

    /// <summary>
    /// Prepares the HTTP client with the base address and authorization headers.
    /// </summary>
    private async Task PrepareAuthenticatedClientAsync()
    {
        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri(_baseAddress);
        }

        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(_scopes);
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(Microsoft.Identity.Web.Constants.Bearer, accessToken);
        _httpClient.AddHeaderAcceptJson();
    }

    /// <summary>
    /// Generates a mock list of addresses for testing purposes.
    /// </summary>
    /// <param name="postcode">The postcode to assign to each mock address.</param>
    /// <returns>A mock list of addresses.</returns>
    private static AddressList GenerateMockAddressList(string postcode, int count = 5)
    {
        var addressList = new AddressList { Addresses = new List<Address>() };

        for (int i = 1; i <= count; i++)
        {
            addressList.Addresses.Add(new Address
            {
                BuildingNumber = i.ToString(),
                Street = "Test Street",
                County = "Test County",
                Locality = "Test Locality",
                Postcode = postcode,
                Town = "Test Town"
            });
        }

        return addressList;
    }

    #endregion
}
