namespace Epr.Reprocessor.Exporter.UI.Sessions;

[ExcludeFromCodeCoverage]
public class JourneySession : IHasUserData
{
	/// <summary>
	/// Data related to the user that is managing on behalf of their org.
	/// </summary>
	public UserData UserData { get; set; } = new();

	/// <summary>
	/// SelectedOrganisationId` is the unique identifier for the organisation that the user has selected.
	/// </summary>
	public Guid? SelectedOrganisationId { get; set; }

	/// <summary>
	/// Data related to ReEx account management to share between services
	/// </summary>
	public ReExAccountManagementSession ReExAccountManagementSession { get; set; } = new();
}
