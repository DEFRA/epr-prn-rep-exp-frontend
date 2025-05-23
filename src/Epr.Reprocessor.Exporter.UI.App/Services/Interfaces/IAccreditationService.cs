﻿using Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;
using EPR.Common.Authorization.Models;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces
{
    public interface IAccreditationService
    {
        Task<AccreditationDto> GetAccreditation(Guid accreditationId);
        Task UpsertAccreditation(AccreditationRequestDto request);
        Task<List<AccreditationPrnIssueAuthDto>> GetAccreditationPrnIssueAuths(Guid accreditationId);
        Task ReplaceAccreditationPrnIssueAuths(Guid accreditationId, List<AccreditationPrnIssueAuthRequestDto> requestDtos);

        Task<IEnumerable<ManageUserDto>> GetOrganisationUsers(EPR.Common.Authorization.Models.Organisation organisation, int serviceRoleId);
        Task<IEnumerable<ManageUserDto>> GetOrganisationUsers(UserData user, bool IncludeLoggedInUser = false);
    }
}