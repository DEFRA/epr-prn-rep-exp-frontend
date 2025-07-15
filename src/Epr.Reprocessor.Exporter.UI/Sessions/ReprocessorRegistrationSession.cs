namespace Epr.Reprocessor.Exporter.UI.Sessions;

/// <summary>
/// Represents details of the current session that is holding details of the registration application.
/// </summary>
[ExcludeFromCodeCoverage]
public class ReprocessorRegistrationSession : IHasUserData, IHasJourneyTracking
{
    /// <summary>
    /// Data related to the user that is registering on behalf of their org.
    /// </summary>
    public UserData UserData { get; set; } = new();

    /// <summary>
	/// SelectedOrganisationId` is the unique identifier for the organisation that the user has selected.
	/// </summary>
	public Guid? SelectedOrganisationId { get; set; }

	/// <summary>
	/// Tracks the journey of pages the user has visited in the registration process.
	/// </summary>
	public List<string> Journey { get; set; } = new();

    /// <summary>
    /// The unique identifier for the registration application.
    /// </summary>
    public Guid? RegistrationId { get; set; }
    
    public ReExAccountManagementSession ReExAccountManagement { get; set; }  = new();

    /// <summary>
    /// Represents details of the registration application.
    /// </summary>
    public RegistrationApplicationSession RegistrationApplicationSession { get; set; } = new();

    //TODO: Check this session in RPD and confirm if we can base our session on it
    //public RegistrationApplicationSession RegistrationApplicationSession { get; set; } = new ();

    /// <summary>
    /// Sets the registration ID.
    /// </summary>
    /// <param name="registrationId">The registration ID to set.</param>
    /// <returns>This instance.</returns>
    public ReprocessorRegistrationSession CreateRegistration(Guid registrationId)
    {
        RegistrationId = registrationId;

        return this;
    }

    /// <summary>
    /// Set session registration from an existing registration.
    /// </summary>
    /// <param name="existingRegistration">The existing registration to set from.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentNullException">Throws if the reprocessing site address is null.</exception>
    public ReprocessorRegistrationSession SetFromExisting(RegistrationDto existingRegistration)
    {
        RegistrationId = existingRegistration.Id;

        RegistrationApplicationSession = new RegistrationApplicationSession
        {
            ReprocessingSite = new ReprocessingSite()
        };

        if (existingRegistration.ReprocessingSiteAddress is null)
        {
            throw new InvalidOperationException(@"The reprocessing site address is null, this should not be null as it's the first save point and you can only have a existing registration entry if it has a populated reprocessing site address.");
        }

        RegistrationApplicationSession.ReprocessingSite = new ReprocessingSite
        {
            SiteGridReference = existingRegistration.ReprocessingSiteAddress.GridReference,
            Address = new Address(
                existingRegistration.ReprocessingSiteAddress.AddressLine1,
                existingRegistration.ReprocessingSiteAddress.AddressLine2,
                null,
                existingRegistration.ReprocessingSiteAddress.TownCity,
                existingRegistration.ReprocessingSiteAddress.County,
                existingRegistration.ReprocessingSiteAddress.Country,
                existingRegistration.ReprocessingSiteAddress.PostCode),
            ServiceOfNotice = new ServiceOfNotice
            {
                Address = new Address(
                    existingRegistration.ReprocessingSiteAddress.AddressLine1,
                    existingRegistration.ReprocessingSiteAddress.AddressLine2,
                    null,
                    existingRegistration.ReprocessingSiteAddress.TownCity,
                    existingRegistration.ReprocessingSiteAddress.County,
                    existingRegistration.ReprocessingSiteAddress.Country,
                    existingRegistration.ReprocessingSiteAddress.PostCode)
            }
        };

        return this;
    }
}