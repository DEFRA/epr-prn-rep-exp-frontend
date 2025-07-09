using Epr.Reprocessor.Exporter.UI.App.DTOs.Submission;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.App.Extensions;
using Epr.Reprocessor.Exporter.UI.App.Options;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System.Net.Http.Headers;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

public class WebApiGatewayClient : IWebApiGatewayClient
{
    private readonly HttpClient _httpClient;
    private readonly string[] _scopes;
    private readonly ILogger<WebApiGatewayClient> _logger;
    private readonly ITokenAcquisition _tokenAcquisition;

    public WebApiGatewayClient(
        ILogger<WebApiGatewayClient> logger,
        HttpClient httpClient,
        ITokenAcquisition tokenAcquisition,
        IOptions<WebApiOptions> webApiOptions
       )
    {
        _logger = logger;
        _httpClient = httpClient;
        _tokenAcquisition = tokenAcquisition;
        _scopes = [webApiOptions.Value.DownstreamScope];
    }

    public async Task<Guid> UploadFileAccreditationAsync(
        byte[] byteArray,
        string fileName,        
        SubmissionType submissionType,
        Guid? submissionId)
    {
        await PrepareAuthenticatedClientAsync();

        _httpClient.AddHeaderFileName(fileName);
        _httpClient.AddHeaderSubmissionType(submissionType);
        _httpClient.AddHeaderSubmissionIdIfNotNull(submissionId);

        var response = await _httpClient.PostAsync("api/v1/file-upload-accreditation", new ByteArrayContent(byteArray));

        response.EnsureSuccessStatusCode();
        var responseLocation = response.Headers.Location.ToString();
        return new Guid(responseLocation.Split('/')[^1]);
    }

    public async Task<byte[]> FileDownloadAsync(string queryString)
    {
        await PrepareAuthenticatedClientAsync();

        try
        {
            var fileResponse = await _httpClient.GetAsync($"api/v1/file-download?{queryString}");

            fileResponse.EnsureSuccessStatusCode();

            var fileData = await fileResponse.Content.ReadAsByteArrayAsync();

            return fileData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Downloading File");
            throw;
        }
    }

    public async Task<T?> GetSubmissionAsync<T>(Guid id)
        where T : AbstractSubmission
    {
        try
        {
            await PrepareAuthenticatedClientAsync();

            var response = await _httpClient.GetAsync($"/api/v1/submissions/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return default;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting submission {Id}", id);
            throw;
        }
    }

    private async Task PrepareAuthenticatedClientAsync()
    {
        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(_scopes);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
           Microsoft.Identity.Web.Constants.Bearer, accessToken);
    }
}
