using EPR.Common.Authorization.Constants;

namespace Epr.Reprocessor.Exporter.UI.App.Constants
{
    public static class UserAccountPaths
    {
        public const string Get = "v1/user-accounts?serviceKey={0}";
        public const string GetPersonByUserId = "persons?userId={0}";
        public const string GetAllPersonByUserId = "persons/all-persons?userId={0}";
        public const string GetUsersByOrganisationOLD = "organisations/users?organisationId={0}&serviceRoleId={1}";
        public const string GetUsersByOrganisation = "organisations/all-users?organisationId={0}";
    }
}