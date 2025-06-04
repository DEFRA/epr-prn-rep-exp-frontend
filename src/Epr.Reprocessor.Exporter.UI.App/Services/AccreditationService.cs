using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.App.Enums.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using EPR.Common.Authorization.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

[ExcludeFromCodeCoverage]
public class AccreditationService(
    IEprFacadeServiceApiClient client,
    IUserAccountService userAccountService,
    ILogger<AccreditationService> logger) : IAccreditationService
{
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

    public async Task<IEnumerable<ManageUserDto>> GetOrganisationUsers(UserData user, bool IncludeLoggedInUser = false)
    {
        ArgumentNullException.ThrowIfNull(user);
        ;
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

    public string CreateApplicationReferenceNumber(string journeyType, int nationId, ApplicationType appType, string organisationNumber, string material)
    {
        string nationCode = nationId switch
        {
            (int)UkNation.England => "E",
            (int)UkNation.Scotland => "S",
            (int)UkNation.Wales => "W",
            (int)UkNation.NorthernIreland => "N",
            _ => String.Empty,
        };
        string applicationCode = appType switch
        {
            ApplicationType.Reprocessor => "R",
            ApplicationType.Exporter => "X",
            ApplicationType.Producer => "P",
            ApplicationType.ComplianceScheme => "C",
            _ => String.Empty,
        };
        string materialCode = material.ToLower() switch
        {
            "aluminium" => "AL",
            "glass" => "GL",
            "steel" => "ST",
            "paper" => "PA",
            "plastic" => "PL",
            "wood" => "WO",
            _ => String.Empty,
        };
        string randomNumber = GenerateRandomNumberFrom1000();

        return $"{journeyType}{DateTime.Today.Year - 2000}{nationCode}{applicationCode}{organisationNumber}{randomNumber}{materialCode}";
    }

    private static string GenerateRandomNumberFrom1000()
    {
        const int MinValue = 1000;

        int randomNumber = RandomNumberGenerator.GetInt32(MinValue, 10 * MinValue);
        return randomNumber.ToString();
    }
}
