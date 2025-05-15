using Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using EPR.Common.Authorization.Models;

namespace Epr.Reprocessor.Exporter.UI.App.Services
{
    public class AccreditationService(IUserAccountService userAccountService) : IAccreditationService
    {

        public async Task<IEnumerable<ManageUserDto>?> GetOrganisationUsers(UserData user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (user.Organisations == null || user.Organisations.Count == 0)
                throw new ArgumentException("User must have at least one organisation.", nameof(user.Organisations));
           
            var users = await userAccountService.GetUsersForOrganisationAsync(user.Organisations.Single().Id.ToString()!, user.ServiceRoleId);

            return users;
        }

        public async Task<IEnumerable<ManageUserDto>?> GetOrganisationUsers(EPR.Common.Authorization.Models.Organisation organisation, int serviceRoleId)
        {
            if (!organisation.Id.HasValue)
                throw new ArgumentNullException(nameof(organisation));
            if (organisation.Id == Guid.Empty)
                throw new ArgumentException("The organisation does not have a valid ID.", nameof(organisation.Id));
            if (serviceRoleId == 0)
                throw new ArgumentException("The service role ID is not valid.", nameof(serviceRoleId));

            var users = await userAccountService.GetUsersForOrganisationAsync(organisation.Id.ToString()!, serviceRoleId);

            return users;
        }
    }
}
