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

        Task<IEnumerable<ManageUserDto>> GetOrganisationUsers(EPR.Common.Authorization.Models.Organisation organisation, int serviceRoleId);

        Task<IEnumerable<ManageUserDto>> GetOrganisationUsers(UserData user, bool includeLoggedInUser = false);

        Task<List<OverseasAccreditationSiteDto>?> GetAllSitesByAccreditationId(Guid accreditationId);

        Task PostSiteByAccreditationId(Guid accreditationId, OverseasAccreditationSiteDto request);

        string CreateApplicationReferenceNumber(ApplicationType appType, string organisationNumber);

        Task ClearDownDatabase();

        Task<AccreditationFileUploadDto?> GetAccreditationFileUpload(Guid externalId);

        Task<List<AccreditationFileUploadDto>> GetAccreditationFileUploads(Guid accreditationId, int fileUploadTypeId, int fileUploadStatusId = (int)AccreditationFileUploadStatus.UploadComplete);

        Task<AccreditationFileUploadDto> UpsertAccreditationFileUpload(Guid accreditationId, AccreditationFileUploadDto request);

        Task DeleteAccreditationFileUpload(Guid accreditationId, Guid fileId);
    }
}