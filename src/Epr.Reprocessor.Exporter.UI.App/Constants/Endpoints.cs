namespace Epr.Reprocessor.Exporter.UI.App.Constants;

public static class Endpoints
{
    public const string GetRegistration = "api/v1/registrations/{0}";
    public const string GetByOrganisation = "api/v1/registrations/{0}/organisation/{1}";
    public const string CreateRegistration = "api/v1/Registrations";
    public const string UpdateRegistrationTaskStatus = "api/v1/Registrations/{registrationId}/TaskStatus";
    public const string UpdateRegistrationSiteAddress = "api/v1/Registrations/{registrationId}/SiteAddress";
}
