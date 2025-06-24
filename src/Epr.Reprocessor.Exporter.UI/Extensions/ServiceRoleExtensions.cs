namespace Epr.Reprocessor.Exporter.UI.Extensions;

public static class ServiceRoleExtensions
{
    public static string GetRoleName(this string roleKey)
    {
        return roleKey switch
        {
            "Packaging.ApprovedPerson" => "Approved Person",
            "Packaging.DelegatedPerson" => "Delegated Person",
            "Packaging.BasicUser" => "Basic User",

            "Regulator.Admin" => "Regulator Admin",
            "Regulator.Basic" => "Regulator Basic",

            "LaPayment.UserAdministrator" => "User Administrator",
            "LaPayment.BasicUser" => "Basic User",

            "Re-Ex.ApprovedPerson" => "Approved Person",
            "Re-Ex.DelegatedPerson" => "Delegated Person",
            "Re-Ex.BasicUser" => "Basic User",
            "Re-Ex.AdminUser" => "Admin User",
            
            "Basic.Employee" => "Basic User",
            "Basic.Admin" => "Admin User",

            _ => roleKey // fallback to raw key if not found
        };
    }
}