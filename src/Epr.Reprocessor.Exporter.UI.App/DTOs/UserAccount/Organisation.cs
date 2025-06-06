using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Newtonsoft.Json;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;

[ExcludeFromCodeCoverage]
public class Organisation
{
    public Guid Id { get; set; }

    [JsonProperty("name")]
    public string OrganisationName { get; set; } = null!;

    public string OrganisationRole { get; set; } = null!;

    public string OrganisationType { get; set; } = null!;

    public string CompaniesHouseNumber { get; set; } = null!;

    public string? SubBuildingName { get; set; }

    public string? BuildingName { get; set; }

    public string? BuildingNumber { get; set; }

    public string? Street { get; set; }

    public string? Locality { get; set; }

    public string? DependentLocality { get; set; }

    public string? Town { get; set; }

    public string? County { get; set; }

    public string? Country { get; set; }

    public string? Postcode { get; set; }

    public string? OrganisationAddress { get; set; }

    public string? JobTitle { get; set; }

    public UkNation NationId { get; set; }

    public string OrganisationNumber { get; set; }

    public List<Enrolment> Enrolments { get; set; } = new();
}