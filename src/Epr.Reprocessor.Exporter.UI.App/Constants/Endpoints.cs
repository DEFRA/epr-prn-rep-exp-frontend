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
        public const string RegistrationTaskStatus = "api/v1/Registrations/{registrationId}/RegistrationTaskStatus";
        public const string GetRegistrationsData = "api/v1/Registrations/{organisationId}/overview";
        public const string GetRegistrationsOverviewByOrgId = "api/v1/Registrations/{organisationId}/overview";
    }
    public static class Lookup
    {
        public const string GetCountries = "api/v1/lookup/countries";
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
        public const string CreateMaterialExemptionReferences = "api/v1/RegistrationMaterials/CreateExemptionReferences";
    }

    public static class  RegistrationMaterial
    {
        public const string CreateRegistrationMaterialAndExemptionReferences = "api/v1/RegistrationMaterials/CreateExemptionReferences";
        public const string CreateRegistrationMaterial = "api/v1/registrationMaterials/CreateRegistrationMaterial";
        public const string UpdateRegistrationMaterial = "api/v1/registrations/{0}/materials/{1}/update";
        public const string UpdateRegistrationMaterialPermits = "api/v1/registrationMaterials/{0}/permits";
        public const string UpsertRegistrationMaterialContact = "api/v1/registrationMaterials/{0}/contact";
        public const string UpsertRegistrationInputsAndOutputs = "api/v1/registrationMaterials/{0}/inputsAndOutputs";
        public const string UpdateRegistrationMaterialPermitCapacity = "api/v1/registrationMaterials/{0}/permitCapacity";
        public const string GetMaterialsPermitTypes = "api/v1/registrationMaterials/permitTypes";
        public const string GetAllRegistrationMaterials = "api/v1/registrationMaterials/{0}/materials";
        public const string Delete = "api/v1/registrationMaterials/{0}";
        public const string SaveOverseasReprocessor = "api/v1/registrationMaterials/SaveOverseasReprocessor";
        public const string UpdateMaximumWeight = "api/v1/registrationMaterials/{0}/max-weight";
        public const string UpdateTaskStatus = "api/v1/registrationMaterials/{0}/task-status";
        public const string UpdateIsMaterialRegistered = "api/v1/registrationMaterials/UpdateIsMaterialRegistered";
        public const string UpdateApplicationRegistrationTaskStatus = "api/v1/Registrations/{registrationMaterialId}/applicationTaskStatus";
        public const string UpsertRegistrationReprocessingDetails = "api/v1/registrationMaterials/{0}/registrationReprocessingDetails";
        public const string UpdateMaterialNotReprocessingReason = "api/v1/registrationMaterials/{0}/materialNotReprocessingReason";
        public const string GetOverseasMaterialReprocessingSites = "api/v{0}/registrationMaterials/{1}/overseasMaterialReprocessingSites";
        public const string SaveInterimSites = "api/v{0}/registrationMaterials/SaveInterimSites";
    }

    public static class ExporterJourney
    {
        public const string OtherPermitsGet = "api/v{0}/ExporterRegistrations/{1}/carrier-broker-dealer-permits";
        public const string OtherPermitsPut = "api/v{0}/ExporterRegistrations/{1}/carrier-broker-dealer-permits";
        public const string WasteCarrierBrokerDealerRefGet = "api/v{0}/ExporterRegistrations/{1}/waste-carrier-broker-dealer-ref";
        public const string WasteCarrierBrokerDealerRefPost = "api/v{0}/ExporterRegistrations/{1}/waste-carrier-broker-dealer-ref";
        public const string WasteCarrierBrokerDealerRefPut = "api/v{0}/ExporterRegistrations/{1}/waste-carrier-broker-dealer-ref";
        public const string LegalAddressGet = "api/v{0}/ExporterRegistrations/{1}/address-for-service-of-notices";
        public const string LegalAddressPut = "api/v{0}/ExporterRegistrations/{1}/address-for-service-of-notices";
    }

    public static class CurrentVersion
    {
        public const string Version = "1";
    }
}