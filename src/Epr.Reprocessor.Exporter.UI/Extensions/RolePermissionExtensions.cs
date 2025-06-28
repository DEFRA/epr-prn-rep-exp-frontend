using Microsoft.AspNetCore.Mvc.Localization;

namespace Epr.Reprocessor.Exporter.UI.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class RolePermissionExtensions
    {
        public static LocalizedHtmlString GetPermissionDescription(this string roleKey, IViewLocalizer localizer)
        {
            if (string.IsNullOrEmpty(roleKey))
                return localizer["ManageOrganisation.TabTeam.Permissions.ReadOnly"];

            var lowerKey = roleKey.ToLowerInvariant();

            if (lowerKey.Contains("adminuser") || lowerKey.Contains("useradministrator"))
            {
                return localizer["ManageOrganisation.TabTeam.Permissions.ManageTeamSubmitAndApply"];
                // e.g. "Manage team, submit registration and apply for accreditation"
            }

            if (lowerKey.Contains("approvedperson"))
            {
                return localizer["ManageOrganisation.TabTeam.Permissions.ManageTeamAndSubmitRegistration"];
                // e.g. "Manage team, submit registration and accreditation"
            }

            if (lowerKey.Contains("delegatedperson") || lowerKey.Contains("standarduser"))
            {
                return localizer["ManageOrganisation.TabTeam.Permissions.SubmitRegistrationAndApply"];
                // e.g. "Submit registration and apply for accreditation"
            }

            return localizer["ManageOrganisation.TabTeam.Permissions.ReadOnly"];
        }
    }
}