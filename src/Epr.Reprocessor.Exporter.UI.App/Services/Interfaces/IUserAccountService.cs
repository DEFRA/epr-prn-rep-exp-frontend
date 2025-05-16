using Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

public interface IUserAccountService
{
    public Task<UserAccountDto?> GetUserAccount();

    public Task<PersonDto?> GetPersonByUserId(Guid userId);
    Task<IEnumerable<ManageUserDto>?> GetUsersForOrganisationAsync(string organisationId, int serviceRoleId);
}