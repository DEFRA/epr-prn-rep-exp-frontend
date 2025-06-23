namespace Epr.Reprocessor.Exporter.UI.Extensions;

public static class ServiceRoleMapper
{
    public static UserServiceRole MapToUserServiceRole(string role)
    {
        return role?.Trim() switch
        {
            "Approved Person" => UserServiceRole.ApprovedPerson,
            "Administrator" => UserServiceRole.Administrator,
            "Standard User" => UserServiceRole.StandardUser,
            "Basic User" => UserServiceRole.BasicUser,
            _ => UserServiceRole.BasicUser // fallback
        };
    }
}