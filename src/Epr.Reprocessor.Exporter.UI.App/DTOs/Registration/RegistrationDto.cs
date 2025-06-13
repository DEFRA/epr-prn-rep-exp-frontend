using Epr.Reprocessor.Exporter.UI.App.Enums.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.Enums.Registration;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

/// <summary>
/// Defines a model for containing registration data.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Its a DTO and doesn't have  any logic")]
public class RegistrationDto
{
    /// <summary>
    /// The unique identifier for the registration.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The type of the application i.e. Reprocessor, Exporter, Producer, or ComplianceScheme.
    /// </summary>
    public ApplicationType ApplicationTypeId { get; set; }
    
    /// <summary>
    /// The unique id associated with the logged-in user and the organisation for the registration.
    /// </summary>
    public Guid OrganisationId { get; set; }
    
    /// <summary>
    /// The current status of the registration.
    /// </summary>
    public RegistrationStatus RegistrationStatus { get; set; }

    /// <summary>
    /// The ID for the business address entity, used to link to the address details.
    /// </summary>
    public int? BusinessAddressId { get; set; }

    /// <summary>
    /// The business address details associated with the registration.
    /// </summary>
    public AddressDto? BusinessAddress { get; set; }

    /// <summary>
    /// The ID for the reprocessing site address entity, used to link to the address details.
    /// </summary>
    public int? ReprocessingSiteAddressId { get; set; }

    /// <summary>
    /// The reprocessing site address details associated with the registration. This is the site whereby the organisation processes waste materials and serves as the core location for the registration.
    /// </summary>
    public AddressDto? ReprocessingSiteAddress { get; set; }

    /// <summary>
    /// The ID for the legal address entity, used to link to the address details.
    /// </summary>
    public int? LegalDocumentAddressId { get; set; }

    /// <summary>
    /// The legal address for the registration, this is where notices will be served to.
    /// </summary>
    public AddressDto? LegalDocumentAddress { get; set; }

    /// <summary>
    /// The ID of the assigned officer for the registration. This is the officer who is responsible for overseeing the registration process.
    /// </summary>
    public int AssignedOfficerId { get; set; }

    /// <summary>
    /// The status for the accreditation.
    /// </summary>
    public AccreditationStatus AccreditationStatus { get; set; }

    /// <summary>
    /// The current year being applied for, this may come into play more should there be a renewal process in the future.
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// The unique identifier for the registration material entry, which is used to link the registration to specific materials being processed or exported.
    /// </summary>
    public int RegistrationMaterialId { get; set; }

    /// <summary>
    /// The unique identifier for the material being registered, which is used to identify the type of material that the registration pertains to.
    /// </summary>
    public int MaterialId { get; set; }

    /// <summary>
    /// The name of the material being registered for.
    /// </summary>
    public string Material { get; set; } = null!;

    /// <summary>
    /// The unique identifier for the reprocessing site.
    /// </summary>
    public int? ReprocessingSiteId { get; set; }
}