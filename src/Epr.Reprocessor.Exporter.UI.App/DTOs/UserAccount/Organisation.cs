using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;

[ExcludeFromCodeCoverage]
public class Organisation
{
    public Guid Id { get; set; }

    [JsonProperty("name")]
    public string OrganisationName { get; set; }

    public string OrganisationRole { get; set; }

    public string OrganisationType { get; set; }
    public string organisationNumber { get; set; }
    public List<Enrolment> enrolments { get; set; } = new();
}