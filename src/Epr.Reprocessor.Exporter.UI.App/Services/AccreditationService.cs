using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;
using Epr.Reprocessor.Exporter.UI.App.Enums.Accreditation;
using EPR.Common.Authorization.Models;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

public class AccreditationService(
    IEprFacadeServiceApiClient client,
    IUserAccountService userAccountService,
    ILogger<AccreditationService> logger) : IAccreditationService
{
    [ExcludeFromCodeCoverage]
    public async Task ClearDownDatabase()
    {
        // Temporary: Aid to QA whilst Accreditation uses in-memory database.
        var result = await client.SendPostRequest<Object>("api/v1.0/Accreditation/clear-down-database", null);
        result.EnsureSuccessStatusCode();
    }
    
    public async Task<Guid> GetOrCreateAccreditation(
        Guid organisationId,
        int materialId,
        int applicationTypeId)
    {
        try
        {
            var result = await client.SendGetRequest($"{EprPrnFacadePaths.Accreditation}/{organisationId}/{materialId}/{applicationTypeId}");
            result.EnsureSuccessStatusCode();

            return await result.Content.ReadFromJsonAsync<Guid>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get or create accreditation - organisationId: {OrganisationId}, materialId: {MaterialId} and applicationTypeId: {ApplicationTypeId}", organisationId, materialId, applicationTypeId);
            throw;
        }
    }

    public async Task<AccreditationDto> GetAccreditation(Guid accreditationId)
    {
        try
        {
            var result = await client.SendGetRequest($"{EprPrnFacadePaths.Accreditation}/{accreditationId}");
            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            result.EnsureSuccessStatusCode();

            return await result.Content.ReadFromJsonAsync<AccreditationDto>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve accreditation - accreditationId: {AccreditationId}", accreditationId);
            throw;
        }
    }

    public async Task UpsertAccreditation(AccreditationRequestDto request)
    {
        try
        {
            var result = await client.SendPostRequest(EprPrnFacadePaths.Accreditation, request);

            result.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to upsert accreditation - accreditationId: {AccreditationId}", request.ExternalId);
            throw;
        }
    }

    public async Task<List<AccreditationPrnIssueAuthDto>> GetAccreditationPrnIssueAuths(Guid accreditationId)
    {
        try
        {
            var result = await client.SendGetRequest($"{EprPrnFacadePaths.AccreditationPrnIssueAuth}/{accreditationId}");
            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            result.EnsureSuccessStatusCode();

            return await result.Content.ReadFromJsonAsync<List<AccreditationPrnIssueAuthDto>>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve AccreditationPrnIssueAuth entities - accreditationId: {AccreditationId}", accreditationId);
            throw;
        }
    }

    public async Task ReplaceAccreditationPrnIssueAuths(Guid accreditationId, List<AccreditationPrnIssueAuthRequestDto> requestDtos)
    {
        try
        {
            var result = await client.SendPostRequest($"{EprPrnFacadePaths.AccreditationPrnIssueAuth}/{accreditationId}", requestDtos);
            result.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update AccreditationPrnIssueAuth entities - accreditationId: {AccreditationId}", accreditationId);
            throw;
        }
    }

    public async Task<AccreditationFileUploadDto?> GetAccreditationFileUpload(Guid externalId)
    {
        try
        {
            var result = await client.SendGetRequest($"{EprPrnFacadePaths.Accreditation}/Files/{externalId}");
            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            result.EnsureSuccessStatusCode();

            return await result.Content.ReadFromJsonAsync<AccreditationFileUploadDto>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve accreditation file upload - externalId: {ExternalId}", externalId);
            throw;
        }
    }

    public async Task<List<AccreditationFileUploadDto>> GetAccreditationFileUploads(Guid accreditationId, int fileUploadTypeId, int fileUploadStatusId = (int)AccreditationFileUploadStatus.UploadComplete)
    {
        try
        {
            var result = await client.SendGetRequest($"{EprPrnFacadePaths.Accreditation}/{accreditationId}/Files/{fileUploadTypeId}/{fileUploadStatusId}");
            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            result.EnsureSuccessStatusCode();

            return await result.Content.ReadFromJsonAsync<List<AccreditationFileUploadDto>>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve accreditation file uploads - accreditationId: {AccreditationId}", accreditationId);
            throw;
        }
    }

    public async Task<AccreditationFileUploadDto> UpsertAccreditationFileUpload(Guid accreditationId, AccreditationFileUploadDto request)
    {
        try
        {
            var result = await client.SendPostRequest($"{EprPrnFacadePaths.Accreditation}/{accreditationId}/Files", request);

            result.EnsureSuccessStatusCode();

            return await result.Content.ReadFromJsonAsync<AccreditationFileUploadDto>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to upsert accreditation file upload - accreditationId: {AccreditationId}, fileId: {FileId}", request.ExternalId, request.FileId);
            throw;
        }
    }

    public async Task DeleteAccreditationFileUpload(Guid accreditationId, Guid fileId)
    {
        try
        {
            var result = await client.SendDeleteRequest($"{EprPrnFacadePaths.Accreditation}/{accreditationId}/Files/{fileId}");

            result.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete accreditation file upload - accreditationId: {AccreditationId}, fileId: {FileId}", accreditationId, fileId);
            throw;
        }
    }

    public async Task<IEnumerable<ManageUserDto>> GetOrganisationUsers(UserData user, bool IncludeLoggedInUser = false)
    {
        ArgumentNullException.ThrowIfNull(user);
        if (user.Organisations == null || user.Organisations.Count == 0)
            throw new ArgumentException("User must have at least one organisation.", nameof(user.Organisations));

        var users = await userAccountService.GetUsersForOrganisationAsync(user.Organisations?.SingleOrDefault()?.Id.ToString(), user.ServiceRoleId);
        if (IncludeLoggedInUser && user.Id.HasValue)
        {
            users = users.Prepend(new ManageUserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PersonId = user.Id.Value,
                ServiceRoleId = user.ServiceRoleId
            });
        }
        return users;
    }

    public async Task<IEnumerable<ManageUserDto>> GetOrganisationUsers(EPR.Common.Authorization.Models.Organisation organisation, int serviceRoleId)
    {
        if (!organisation.Id.HasValue)
            throw new ArgumentNullException(nameof(organisation));
        if (organisation.Id == Guid.Empty)
            throw new ArgumentException("The organisation does not have a valid ID.", nameof(organisation.Id));
        if (serviceRoleId == 0)
            throw new ArgumentException("The service role ID is not valid.", nameof(serviceRoleId));

        var users = await userAccountService.GetUsersForOrganisationAsync(organisation?.Id.ToString(), serviceRoleId);

        return users;
    }

    public async Task<List<OverseasAccreditationSiteDto>?> GetAllSitesByAccreditationId(Guid accreditationId)
    {
        try
        {
            var result = await client.SendGetRequest($"{EprPrnFacadePaths.OverseasAccreditationSite}/{accreditationId}");
            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            result.EnsureSuccessStatusCode();

            return await result.Content.ReadFromJsonAsync<List<OverseasAccreditationSiteDto>>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve OverseasAccreditationSiteDtos for accreditationId {AccreditationId}", accreditationId);
            throw;
        }
    }

    public async Task PostSiteByAccreditationId(Guid accreditationId, OverseasAccreditationSiteDto request)
    {
        try
        {
            var result = await client.SendPostRequest($"{EprPrnFacadePaths.OverseasAccreditationSite}/{accreditationId}", request);

            result.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to post OverseasAccreditationSiteDto for accreditationId {AccreditationId}", accreditationId);
            throw;
        }
    }

    public string CreateApplicationReferenceNumber(ApplicationType appType, string organisationNumber)
    {
        string applicationCode = appType switch
        {
            ApplicationType.Reprocessor => "REP",
            ApplicationType.Exporter => "EXP",
            _ => String.Empty,
        };

        return $"PR/PK/{applicationCode}-A{organisationNumber}";
    }
}
