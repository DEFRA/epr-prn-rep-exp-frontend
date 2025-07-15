using Epr.Reprocessor.Exporter.UI.ViewModels.Team;

namespace Epr.Reprocessor.Exporter.UI.Sessions;

[ExcludeFromCodeCoverage]
public class ReExAccountManagementSession
{
	public List<string> Journey { get; set; } = [];

    public Guid PersonId { get; set; } = Guid.Empty;

    public Guid OrganisationId { get; set; } = Guid.Empty;

    public string? OrganisationName { get; set; }

    public string InviteeEmailAddress { get; set; } = default!;

    public string RoleKey { get; set; } = default!;

    public TeamViewModel? TeamViewModel { get; set; }

    public RemoveUserJourneyModel? ReExRemoveUserJourney { get; set; }

	public RemoveUserJourneyModel? RemoveUserJourney { get; set; }
}