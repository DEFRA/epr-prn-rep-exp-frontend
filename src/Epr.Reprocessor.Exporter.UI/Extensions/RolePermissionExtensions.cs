using Microsoft.AspNetCore.Mvc.Localization;

namespace Epr.Reprocessor.Exporter.UI.Extensions
{
    public static class RolePermissionExtensions
    {
        public static LocalizedHtmlString GetPermissionDescription(this string roleKey, IViewLocalizer localizer)
        {
            if (string.IsNullOrEmpty(roleKey)) return localizer["ManageOrganisation.TabTeam.Permissions.ReadOnly"];

            var lowerKey = roleKey.ToLowerInvariant();

            if (lowerKey.Contains("approvedperson") || lowerKey.Contains("adminuser") || lowerKey.Contains("useradministrator"))
            {
                return localizer["ManageOrganisation.TabTeam.Permissions.ManageTeamAndSubmitRegistration"];
            }

            if (lowerKey.Contains("basicuser"))
            {
                return localizer["ManageOrganisation.TabTeam.Permissions.ReadOnly"];
            }

            if (lowerKey.Contains("delegatedperson"))
            {
                return localizer["ManageOrganisation.TabTeam.Permissions.SubmitRegistration"];
            }

            return localizer["ManageOrganisation.TabTeam.Permissions.ReadOnly"];
        }
    }
}