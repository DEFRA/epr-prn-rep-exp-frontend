namespace Epr.Reprocessor.Exporter.UI.App.Services
{
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Http;
    using Azure;
    using Constants;
    using DTOs.UserAccount;
    using Epr.Reprocessor.Exporter.UI.App.Extensions;
    using Interfaces;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    public class UserAccountService : IUserAccountService
    {
        private const string GetUserAccountErrorMessage = "Attempting to get user account failed";
        private readonly ILogger<UserAccountService> _logger;
        private readonly IAccountServiceApiClient _accountServiceApiClient;

        public UserAccountService(IAccountServiceApiClient accountServiceApiClient, ILogger<UserAccountService> logger)
        {
            _logger = logger;
            _accountServiceApiClient = accountServiceApiClient;
        }

        /// <LocalBaseURL> https://localhost:7253/api/ .</LocalBaseURL>
        /// <Endpoint> user-accounts/ .</Endpoint>
        /// <FullLocalUrl> https://localhost:7253/api/user-accounts .</FullLocalUrl>
        /// <summary>Gets a users account.</summary>
        /// <returns>UserAccountDto.</returns>
        public async Task<UserAccountDto?> GetUserAccount(string serviceKey)
        {
            try
            {
                var result = await _accountServiceApiClient.SendGetRequest(string.Format(UserAccountPaths.Get, serviceKey));
                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                result.EnsureSuccessStatusCode();
                var content = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserAccountDto>(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GetUserAccountErrorMessage);
                throw;
            }
        }

        public async Task<PersonDto> GetPersonByUserId(Guid userId)
        {
            try
            {
                return await GetPersonByUserIdByUrl(userId, UserAccountPaths.GetPersonByUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GetUserAccountErrorMessage);
                throw;
            }
        }

        /// <summary>
        /// Gets a person by the UserId. Includes soft-deleted person records too.
        /// </summary>
        /// <param name="userId">UserId of the person to get.</param>
        /// <returns></returns>
        public async Task<PersonDto> GetAllPersonByUserId(Guid userId)
        {
            try
            {
                return await GetPersonByUserIdByUrl(userId, UserAccountPaths.GetAllPersonByUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GetUserAccountErrorMessage);
                throw;
            }
        }

        private async Task<PersonDto> GetPersonByUserIdByUrl(Guid userId, string userAccountPath)
        {
            var result = await _accountServiceApiClient.SendGetRequest(string.Format(userAccountPath, userId));
            result.EnsureSuccessStatusCode();
            var content = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PersonDto>(content);
        }


        /// <summary>
        /// Gets an enumerable of user Information based on the organisation ID and service role ID.
        /// </summary>
        /// <param name="organisationId">The Organisation GUID.</param>
        /// <param name="serviceRoleId">The service role ID.</param>
        /// <returns></returns>
        public async Task<IEnumerable<ManageUserDto>?> GetUsersForOrganisationAsync(string organisationId, int serviceRoleId)
        {
            try
            {
                var result = await _accountServiceApiClient.SendGetRequest(string.Format(UserAccountPaths.GetUsersByOrganisation, organisationId, serviceRoleId));

                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                result.EnsureSuccessStatusCode();
                return await result.Content.ReadFromJsonWithEnumsAsync<IEnumerable<ManageUserDto>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GetUserAccountErrorMessage);
                throw;
            }
        }


    }
}