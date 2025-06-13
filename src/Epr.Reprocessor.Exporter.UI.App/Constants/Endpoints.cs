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
        public const string CreateRegistration = "api/v1/Registrations";
        public const string UpdateRegistrationTaskStatus = "api/v1/Registrations/{registrationId}/TaskStatus";
        public const string UpdateRegistrationSiteAddress = "api/v1/Registrations/{registrationId}/SiteAddress";
        public const string RegistrationTaskStatus = "api/v1/Registrations/{registrationId}/RegistrationTaskStatus";
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
        public const string CreateMaterialExemptionReferences = "api/v1/MaterialExemptionReferences";
    }

    public static class  RegistrationMaterial
    {
        public const string CreateRegistrationMaterialAndExemptionReferences = "api/v1/RegistrationMaterial/CreateRegistrationMaterialAndExemptionReferences";
    }
}

