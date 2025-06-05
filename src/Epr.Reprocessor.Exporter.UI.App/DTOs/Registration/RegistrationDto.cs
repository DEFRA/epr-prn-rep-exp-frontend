using Epr.Reprocessor.Exporter.UI.App.Enums.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.Enums.Registration;
using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

[ExcludeFromCodeCoverage(Justification = "Its a DTO and doesn't have  any logic")]
public class RegistrationDto
{
    public Guid RegistrationId { get; set; }
    public int RegistrationMaterialId { get; set; }
    public int MaterialId { get; set; }
    public string Material { get; set; }
    public int ApplicationTypeId { get; set; }
    public string ApplicationType { get; set; }
    public RegistrationStatus RegistrationStatus { get; set; }
    public AccreditationStatus AccreditationStatus { get; set; }
    public int? ReprocessingSiteId { get; set; }
    public AddressDto? ReprocessingSiteAddress { get; set; }
    public int Year { get; set; }
}