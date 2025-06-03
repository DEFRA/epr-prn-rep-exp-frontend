using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;

[ExcludeFromCodeCoverage]
public class AccreditationDto
{
    public Guid ExternalId { get; set; }
    public Guid OrganisationId { get; set; }
    public int RegistrationMaterialId { get; set; }
    public string MaterialName { get; set; }
    public int ApplicationTypeId { get; set; }
    public int AccreditationStatusId { get; set; }
    public string DecFullName { get; set; }
    public string DecJobTitle { get; set; }
    public string AccreferenceNumber { get; set; }
    public int? AccreditationYear { get; set; }
    public int? PrnTonnage { get; set; }
    public bool PrnTonnageAndAuthoritiesConfirmed { get; set; }
    public decimal? InfrastructurePercentage { get; set; }
    public decimal? PackagingWastePercentage { get; set; }
    public decimal? BusinessCollectionsPercentage { get; set; }
    public decimal? NewUsesPercentage { get; set; }
    public decimal? NewMarketsPercentage { get; set; }
    public decimal? CommunicationsPercentage { get; set; }
    public decimal? OtherPercentage { get; set; }
    public string InfrastructureNotes { get; set; }
    public string PackagingWasteNotes { get; set; }
    public string BusinessCollectionsNotes { get; set; }
    public string NewUsesNotes { get; set; }
    public string NewMarketsNotes { get; set; }
    public string CommunicationsNotes { get; set; }
    public string OtherNotes { get; set; }
}