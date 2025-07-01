namespace Epr.Reprocessor.Exporter.UI.Sessions;

[ExcludeFromCodeCoverage]
public class ExporterRegistrationSession : IHasUserData, IHasJourneyTracking
{
    public UserData UserData { get; set; } = new();

    public List<string> Journey { get; set; } = new();

    public Guid? RegistrationId { get; set; }


    public ExporterRegistrationApplicationSession ExporterRegistrationApplicationSession { get; set; } = new();

    public ExporterRegistrationSession CreateRegistration(Guid registrationId)
    {
        RegistrationId = registrationId;

        return this;
    }    
}