using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

[ExcludeFromCodeCoverage(Justification = "Its a DTO and doesn't have  any logic")]
public class RegistrationDto
{
    public int Id { get; set; }
    public int ApplicationTypeId { get; set; }
    public Guid OrganisationId { get; set; }
    public int RegistrationStatusId { get; set; }

    public int? BusinessAddressId { get; set; }

    public AddressDto? BusinessAddress { get; set; }
    
    public int? ReprocessingSiteAddressId { get; set; }

    public AddressDto? ReprocessingSiteAddress { get; set; }

    public int? LegalDocumentAddressId { get; set; }

    public AddressDto? LegalDocumentAddress { get; set; }

    public int AssignedOfficerId { get; set; }

}