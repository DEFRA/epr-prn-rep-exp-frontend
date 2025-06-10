using Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;
using Epr.Reprocessor.Exporter.UI.App.Enums.Accreditation;
using EPR.Common.Authorization.Models;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces
{
    public interface IAccreditationService
    {
        Task<Guid> GetOrCreateAccreditation(
        Guid organisationId,
        int materialId,
        int applicationTypeId);

        Task<AccreditationDto> GetAccreditation(Guid accreditationId);

        Task UpsertAccreditation(AccreditationRequestDto request);

        Task<List<AccreditationPrnIssueAuthDto>> GetAccreditationPrnIssueAuths(Guid accreditationId);

        Task ReplaceAccreditationPrnIssueAuths(Guid accreditationId, List<AccreditationPrnIssueAuthRequestDto> requestDtos);

        Task<IEnumerable<ManageUserDto>> GetOrganisationUsers(EPR.Common.Authorization.Models.Organisation organisation);

        Task<IEnumerable<ManageUserDto>> GetOrganisationUsers(UserData user, bool IncludeLoggedInUser = false);

        string CreateApplicationReferenceNumber(string journeyType, int nationId, ApplicationType appType, string organisationNumber, string material);

        Task ClearDownDatabase();
    }
}