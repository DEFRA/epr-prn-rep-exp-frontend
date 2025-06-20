using System.Reflection.Metadata;

namespace Epr.Reprocessor.Exporter.UI.App.Constants;

/// <summary>
/// Defines endpoint urls for the underlying Api calls.
/// </summary>
public static class Endpoints
{
    /// <summary>
    /// Defines Api endpoints for the registration Api.
    /// </summary>
    public static class Registration
    {
        public const string GetRegistration = "api/v1/registrations/{0}";
        public const string GetByOrganisation = "api/v1/registrations/{0}/organisations/{1}";
        public const string CreateRegistration = "api/v1/Registrations";
        public const string UpdateRegistration = "api/v1/registrations/{0}/update";
        public const string UpdateRegistrationTaskStatus = "api/v1/Registrations/{registrationId}/TaskStatus";
        public const string UpdateRegistrationSiteAddress = "api/v1/Registrations/{registrationId}/SiteAddress";
    }

    /// <summary>
    /// Defines Api endpoints for the material Api.
    /// </summary>
    public static class Material
    {
        public const string GetAllMaterials = "api/v1/materials";
    }

    public static class MaterialExemptionReference
    {
        public const string CreateMaterialExemptionReferences = "api/v1/RegistrationMaterial/CreateExemptionReferences";
    }

    public static class  RegistrationMaterial
    {
        public const string CreateRegistrationMaterialAndExemptionReferences = "api/v1/RegistrationMaterial/CreateRegistrationMaterialAndExemptionReferences";
        public const string CreateRegistrationMaterial = "api/v1/registrationMaterial/CreateRegistrationMaterial";
        public const string UpdateRegistrationMaterial = "api/v1/registrations/{0}/materials/{1}/update";
        public const string UpdateRegistrationMaterialPermits = "api/v1/registrationMaterials/{0}/permits";
        public const string GetMaterialsPermitTypes = "api/v1/registrationMaterials/permitTypes";
        public const string GetAllRegistrationMaterials = "api/v1/registrationMaterial/{0}/materials";
        public const string Delete = "api/v1/registrationMaterial/{0}";
    }
}