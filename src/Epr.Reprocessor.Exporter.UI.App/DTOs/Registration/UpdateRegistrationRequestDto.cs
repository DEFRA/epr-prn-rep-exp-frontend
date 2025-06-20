using Epr.Reprocessor.Exporter.UI.App.Enums.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.Enums.Registration;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

/// <summary>
/// A request DTO for updating a registration.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateRegistrationRequestDto
{
    /// <summary>
    /// The unique identifier for the registration.
    /// </summary>
    public required Guid RegistrationId { get; set; }

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
    /// Gets or sets the business address
    /// </summary>
    public AddressDto BusinessAddress { get; set; } = new();

    /// <summary>
    /// Gets or sets the reprocessing site address
    /// </summary>
    public AddressDto ReprocessingSiteAddress { get; set; } = new();

    /// <summary>
    /// Gets or sets the legal address
    /// </summary>
    public AddressDto LegalAddress { get; set; } = new();
}