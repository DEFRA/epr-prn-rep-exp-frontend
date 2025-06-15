namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

/// <summary>
/// Defines a DTO for when creating a registration.
/// </summary>
[ExcludeFromCodeCoverage]
public record CreateRegistrationDto
{
    /// <summary>
    /// The application type for the registration, i.e. reprocessor.
    /// </summary>
    public int ApplicationTypeId { get; set; }

    /// <summary>
    /// The unique ID of the organisation that is registering.
    /// </summary>
    public Guid OrganisationId { get; set; }

    /// <summary>
    /// The reprocessing site address for the registration.
    /// </summary>
    public AddressDto ReprocessingSiteAddress { get; set; } = new();
}