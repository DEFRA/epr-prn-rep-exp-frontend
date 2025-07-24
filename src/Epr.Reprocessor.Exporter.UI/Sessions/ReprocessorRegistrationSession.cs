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
    /// <param name="isARegisteredCompany">Determines whether the organisation is a registered company, helps to work out the type of address as we don't store this anywhere so need to work it out each time.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentNullException">Throws if the reprocessing site address is null.</exception>
    public ReprocessorRegistrationSession SetFromExisting(RegistrationDto existingRegistration, bool isARegisteredCompany)
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
            TypeOfAddress = ResolveReprocessingTypeOfAddress(existingRegistration, isARegisteredCompany)
        };

        if (existingRegistration.LegalDocumentAddress is not null)
        {
            RegistrationApplicationSession.ReprocessingSite.ServiceOfNotice = new ServiceOfNotice();
            RegistrationApplicationSession.ReprocessingSite.ServiceOfNotice.SetAddress(
                new Address(
                    existingRegistration.LegalDocumentAddress.AddressLine1,
                    existingRegistration.LegalDocumentAddress.AddressLine2,
                    null,
                    existingRegistration.LegalDocumentAddress.TownCity,
                    existingRegistration.LegalDocumentAddress.County,
                    existingRegistration.LegalDocumentAddress.Country,
                    existingRegistration.LegalDocumentAddress.PostCode),
                ResolveServiceOfNoticeTypeOfAddress(existingRegistration, isARegisteredCompany));
        }

        return this;
    }

    private static AddressOptions ResolveReprocessingTypeOfAddress(RegistrationDto existingRegistration, bool isRegistered)
    {
        AddressOptions type;
        if (existingRegistration.ReprocessingSiteAddress == existingRegistration.BusinessAddress)
        {
            type = isRegistered ? AddressOptions.RegisteredAddress : AddressOptions.BusinessAddress;
        }
        else
        {
            type = AddressOptions.DifferentAddress;
        }

        return type;
    }

    private static AddressOptions ResolveServiceOfNoticeTypeOfAddress(RegistrationDto existingRegistration, bool isRegistered)
    {
        AddressOptions type;
        if (existingRegistration.LegalDocumentAddress == existingRegistration.BusinessAddress)
        {
            type = isRegistered ? AddressOptions.RegisteredAddress : AddressOptions.BusinessAddress;
        }
        else if (existingRegistration.LegalDocumentAddress == existingRegistration.ReprocessingSiteAddress)
        {
            type = AddressOptions.SiteAddress;
        }
        else
        {
            type = AddressOptions.DifferentAddress;
        }

        return type;
    }
}