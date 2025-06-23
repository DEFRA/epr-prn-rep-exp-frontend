using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Epr.Reprocessor.Exporter.UI.Extensions
{
    public static class UserServiceRoleStringExtensions
    {
        public static LocalizedHtmlString GetPermissionDescription(this string role, IViewLocalizer localizer)
        {
            return role?.Trim() switch
            {
                "Administrator" => localizer["ManageOrganisation.TabTeam.Permissions.Administrator"],
                "Approved Person" => localizer["ManageOrganisation.TabTeam.Permissions.ApprovedPerson"],
                "Standard User" => localizer["ManageOrganisation.TabTeam.Permissions.StandardUser"],
                _ => localizer["ManageOrganisation.TabTeam.Permissions.BasicUser"]
            };
        }
    }
}