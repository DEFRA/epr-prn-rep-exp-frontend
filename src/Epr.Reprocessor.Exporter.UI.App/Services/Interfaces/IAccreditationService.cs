using Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;
using EPR.Common.Authorization.Models;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces
{
    public interface IAccreditationService
    {
        Task<IEnumerable<ManageUserDto>> GetOrganisationUsers(EPR.Common.Authorization.Models.Organisation organisation, int serviceRoleId);
        Task<IEnumerable<ManageUserDto>> GetOrganisationUsers(UserData user);
    }
}