using System.Text.Json;
using Azure.Core;
using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;
using System.Net;
using Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using EPR.Common.Authorization.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

[ExcludeFromCodeCoverage]
public class AccreditationService(
    IEprFacadeServiceApiClient client,
    IUserAccountService userAccountService,
    ILogger<AccreditationService> logger) : IAccreditationService
{
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

    public async Task<IEnumerable<ManageUserDto>> GetOrganisationUsers(UserData user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));
        if (user.Organisations == null || user.Organisations.Count == 0)
            throw new ArgumentException("User must have at least one organisation.", nameof(user.Organisations));

        var users = await userAccountService.GetUsersForOrganisationAsync(user.Organisations?.SingleOrDefault()?.Id.ToString(), user.ServiceRoleId);

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
}
