using Microsoft.Extensions.Localization;

namespace Epr.Reprocessor.Exporter.UI.Extensions;

public static class UserServiceRoleExtensions
{
    public static string GetPermissionDescription(this UserServiceRole role, IStringLocalizer localizer)
    {
        return role switch
        {
            UserServiceRole.Administrator => localizer["ManageOrganisation.TabTeam.Permissions.Administrator"],
            UserServiceRole.ApprovedPerson => localizer["ManageOrganisation.TabTeam.Permissions.ApprovedPerson"],
            UserServiceRole.StandardUser => localizer["ManageOrganisation.TabTeam.Permissions.StandardUser"],
            UserServiceRole.BasicUser => localizer["Permissions.BasicUser"],
            _ => localizer["ManageOrganisation.TabTeam.Permissions.BasicUser"]
        };
    }
}

