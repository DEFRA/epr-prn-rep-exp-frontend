namespace Epr.Reprocessor.Exporter.UI.App.Constants;

public class PagePaths
{
    public const string ManageOrganisation = "manage-organisation";
    public const string AddOrganisation = "add-organisation-details";
    public const string SelectOrganisation = "select-organisation";
    public const string Culture = "culture"; 
    public const string Privacy = "privacy";
    public const string Cookies = "cookies";
    public const string UpdateCookieAcceptance = "update-cookie-acceptance";
    public const string AcknowledgeCookieAcceptance = "acknowledge-cookie-acceptance";
    public const string CountryOfReprocessingSite = "country-of-reprocessing-site";
    public const string RegistrationLanding = "registration";
    public const string AccreditationLanding = "accreditation";
    public const string ApplicationSaved = "application-saved";
    public const string GridReferenceForEnteredReprocessingSite = "grid-reference-for-entered-reprocessing-site";
    public const string CheckYourAnswersForContactDetails = "check-your-answers-for-contact-details";
    public const string GridReferenceOfReprocessingSite = "grid-reference-of-reprocessing-site";
    public const string CheckAnswers = "check-your-answers-for-contact-details";

    public const string PostcodeOfReprocessingSite = "postcode-of-reprocessing-site";
    public const string EnterReprocessingSiteAddress = "enter-reprocessing-site-address";
    public const string AddressForLegalDocuments = "address-for-legal-documents";
    public const string AddressOfReprocessingSite = "address-of-reprocessing-site";
    public const string SelectAddressOfReprocessingSite = "select-address-of-reprocessing-site";
    public const string ManualAddressForReprocessingSite = "enter-reprocessing-site-address";
    public const string SelectAddressForReprocessingSite = "select-address-of-reprocessing-site";

    public const string AddressForNotices = "address-for-notices";
    public const string PostcodeForServiceOfNotices = "postcode-for-notices";
    public const string SelectAddressForServiceOfNotices = "select-address-for-notices";
    public const string ManualAddressForServiceOfNotices = "enter-address-for-notices";
    public const string ConfirmNoticesAddress = "confirm-address-for-notices";

    public const string NoAddressFound = "no-address-found";


    public const string WastePermitExemptions = "select-materials-authorised-to-recycle";
    public const string PpcPermit = "ppc-permit";
    public const string InstallationPermit = "installation-permit";
    public const string EnvironmentalPermitOrWasteManagementLicence = "environmental-permit-or-waste-management-licence";
    public const string ExemptionReferences = "exemption-references";

    public const string TaskList = "reprocessor-registration-task-list";
    public const string PermitForRecycleWaste = "permit-for-recycling-waste";
    public const string WasteManagementLicense = "waste-management-licence";
    public const string MaximumWeightSiteCanReprocess = "maximum-weight-the-site-can-reprocess";
    public const string RegistrationConfirmation = "confirmation";

    public const string OverseasSiteDetails = "overseas-site-details";
    public const string BaselConventionAndOECDCodes = "basel-OECD-codes";
    public const string AddAnotherOverseasReprocessingSite = "add-another-overseas-reprocessing-site";
    public const string CheckYourAnswersForOverseasProcessingSite = "check-your-answers-overseas-reprocessor";
    public const string DeleteOverseasReprocessingSite = "delete-overseas-reprocessing-site";
    public const string ChangeOverseasReprocessingSite = "change-overseas-reprocessing-site";
    public const string ChangeBaselConvention = "change-basel-convention";

    // Accreditation
    public const string EnsureAccreditation = "ensure-accreditation/{materialId}/{applicationTypeId}";
    public const string NotAnApprovedPerson = "complete-not-submit-accreditation-application";
    public const string CalendarYear = "calendar-year";
    public const string SelectPrnTonnage = "prns-plan-to-issue/{accreditationId}";
    public const string SelectPernTonnage = "perns-plan-to-issue/{accreditationId}";
    public const string CheckAnswersPRNs = "check-your-answers-for-prn-tonnage-and-authority/{accreditationId}";
    public const string CheckAnswersPERNs = "check-your-answers-for-pern-tonnage-and-authority/{accreditationId}";
    public const string BusinessPlanPercentages = "business-plan-percentages/{accreditationId}";
    public const string SelectAuthorityPRNs = "authority-to-issue-prns/{accreditationId}";
    public const string SelectAuthorityPERNs = "authority-to-issue-perns/{accreditationId}";
    public const string ApplyForAccreditation = "apply-for-accreditation";
    public const string AccreditationTaskList = "reprocessor-accreditation-task-list/{accreditationId}";
    public const string ExporterAccreditationTaskList = "exporter-accreditation-task-list/{accreditationId}";
    public const string CheckBusinessPlanPRN = "check-business-plan/{accreditationId}";
    public const string CheckBusinessPlanPERN = "check-business-plan-pern/{accreditationId}";
    public const string MoreDetailOnBusinessPlanPRNs = "detail-about-PRN-spend/{accreditationId}";
    public const string MoreDetailOnBusinessPlanPERNs = "detail-about-PERN-spend/{accreditationId}";
    public const string AccreditationSamplingAndInspectionPlan = "sampling-and-inspection-plan";
    public const string AccreditationDeclaration = "accreditation-application-declaration/{accreditationId}";
    public const string SelectOverseasSites = "select-overseas-sites/{accreditationId}";
    public const string CheckOverseasSites = "confirm-overseas-sites/{accreditationId}";
    public const string ReprocessorAccreditationSamplingFileUpload = "reprocessor-accreditation-sampling-file-upload";
    public const string ExporterAccreditationSamplingFileUpload = "exporter-accreditation-sampling-file-upload";
    public const string ApplyingFor2026Accreditation = "applying-for-2026-accreditation/{accreditationId}";
    public const string ReprocessorApplicationSubmissionConfirmation = "reprocessor-application-submitted/{accreditationId}";
    public const string ExporterApplicationSubmissionConfirmation = "exporter-application-submitted/{accreditationId}";
    public const string UploadEvidenceOfEquivalentStandards = "upload-evidence-of-equivalent-standards/{accreditationId}";
    public const string OptionalUploadOfEvidenceOfEquivalentStandards = "optional-upload-of-evidence-of-equivalent-standards/{accreditationId}";
    public const string EvidenceOfEquivalentStandardsUploadDocument = "evidence-of-equivalent-standards-upload-document";
    public const string EvidenceOfEquivalentStandardsCheckYourAnswers = "evidence-of-equivalent-standards-check-your-answers/{orgName}/{addrLine1}/{addrLine2}/{addrLine3}";
    public const string EvidenceOfEquivalentStandardsCheckSiteFulfillsConditions = "evidence-of-equivalent-standards-check-site-fulfills-conditions/{orgName}/{addrLine1}/{addrLine2}/{addrLine3}";
    public const string EvidenceOfEquivalentStandardsMoreEvidence = "evidence-of-equivalent-standards-more-evidence";
    public const string EvidenceOfEquivalentStandardsCheckYourEvidenceAnswers = "evidence-of-equivalent-standards-check-your-evidence-answers";

    // Use only for pages that have not been developed further than the current page being worked on.
    public const string Placeholder = "placeholder";
}
