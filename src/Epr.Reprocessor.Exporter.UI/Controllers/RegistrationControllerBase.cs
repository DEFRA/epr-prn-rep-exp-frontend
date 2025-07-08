using Epr.Reprocessor.Exporter.UI.Mapper;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

/// <summary>
/// Base controller for registrations.
/// <remarks>A move towards breaking up the RegistrationController.</remarks>
/// </summary>
public class RegistrationControllerBase : Controller
{
    protected const string SaveAndContinueActionKey = "SaveAndContinue";
    protected const string SaveAndComeBackLaterActionKey = "SaveAndComeBackLater";

    #region Constructor

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="sessionManager">Provides access to a session manager.</param>
    /// <param name="reprocessorService">Provides access to a service to handle reprocessor registrations.</param>
    /// <param name="postcodeLookupService">Provides access to a service for looking up addresses and postcodes.</param>
    /// <param name="validationService">Provides access to a service for validating objects.</param>
    /// <param name="requestMapper">Provides access to a mapper.</param>
    public RegistrationControllerBase(
        ISessionManager<ReprocessorRegistrationSession> sessionManager,
        IReprocessorService reprocessorService,
        IPostcodeLookupService postcodeLookupService,
        IValidationService validationService,
        IRequestMapper requestMapper)
    {
        SessionManager = sessionManager;
        ReprocessorService = reprocessorService;
        PostcodeLookupService = postcodeLookupService;
        ValidationService = validationService;
        RequestMapper = requestMapper;
    }

    #endregion

    #region Properties

    /// <summary>
    /// A session manager to manage session objects.
    /// </summary>
    protected ISessionManager<ReprocessorRegistrationSession> SessionManager { get; }

    /// <summary>
    /// A service to manage reprocessor registrations.
    /// </summary>
    protected IReprocessorService ReprocessorService { get; }

    /// <summary>
    /// A service for looking up addresses and postcodes.
    /// </summary>
    protected IPostcodeLookupService PostcodeLookupService { get; }

    /// <summary>
    /// A service for performing validation of objects.
    /// </summary>
    protected IValidationService ValidationService { get; }

    /// <summary>
    /// A string localizer for managing string resources related to authorisation types.
    /// </summary>
    protected IStringLocalizer<SelectAuthorisationType> SelectAuthorisationStringLocalizer { get; }

    /// <summary>
    /// A mapper for mapping requests related to registrations.
    /// </summary>
    protected IRequestMapper RequestMapper { get; }

    #endregion

    #region Base Methods

    /// <summary>
    /// Creates a new registration if one does not exist.
    /// </summary>
    /// <returns>The completed task.</returns>
    [ExcludeFromCodeCoverage(Justification = "TODO: Unit tests to be added as part of create registration user story")]
    protected async Task CreateRegistrationIfNotExistsAsync()
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);

        if (session!.RegistrationId is null)
        {
            var request = await RequestMapper.MapForCreate();
            var registration = await ReprocessorService.Registrations.CreateAsync(request);

            session.CreateRegistration(registration.Id);
        }

        await SessionManager.SaveSessionAsync(HttpContext.Session, session);
    }

    /// <summary>
    /// Creates a new registration if one does not exist.
    /// </summary>
    /// <returns>The completed task.</returns>
    [ExcludeFromCodeCoverage(Justification = "TODO: Unit tests to be added as part of create registration user story")]
    protected async Task UpdateRegistrationAsync()
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session);

        if (session!.RegistrationId is not null)
        {
            var request = await RequestMapper.MapForUpdate();
            await ReprocessorService.Registrations.UpdateAsync(session.RegistrationId!.Value, request);
        }

        await SessionManager.SaveSessionAsync(HttpContext.Session, session);
    }

    /// <summary>
    /// Sets the back link for the current page in the session's journey.
    /// </summary>
    /// <param name="session">The session object.</param>
    /// <param name="currentPagePath">The current page in the journey.</param>
    protected void SetBackLink(ReprocessorRegistrationSession session, string currentPagePath)
    {
        ViewBag.BackLinkToDisplay = session.Journey!.PreviousOrDefault(currentPagePath) ?? string.Empty;
    }

    /// <summary>
    /// Save the current session.
    /// </summary>
    /// <param name="session">The session object.</param>
    /// <param name="currentPagePath">The current page in the journey.</param>
    /// <returns>The completed task.</returns>
    protected async Task SaveSession(ReprocessorRegistrationSession session, string currentPagePath)
    {
        ClearRestOfJourney(session, currentPagePath);

        await SessionManager.SaveSessionAsync(HttpContext.Session, session);
    }

    /// <summary>
    /// Clears the journey in the session from the current page onwards, effectively resetting the journey for the user.
    /// </summary>
    /// <param name="session"></param>
    /// <param name="currentPagePath"></param>
    protected static void ClearRestOfJourney(ReprocessorRegistrationSession session, string currentPagePath)
    {
        var index = session.Journey.IndexOf(currentPagePath);

        // this also cover if current page not found (index = -1) then it clears all pages
        session.Journey = session.Journey.Take(index + 1).ToList();
    }

    /// <summary>
    /// Temporary method that retrieves stub data from the TempData dictionary.
    /// </summary>
    /// <typeparam name="T">The generic type parameter.</typeparam>
    /// <param name="key">The key for the temp data object.</param>
    /// <returns>The object instance deserialized to <see cref="T"/>.</returns>
    protected T? GetStubDataFromTempData<T>(string key) where T : new()
    {
        TempData.TryGetValue(key, out var tempData);
        if (tempData is not null)
        {
            TempData.Clear();
            return JsonConvert.DeserializeObject<T>(tempData.ToString()!);
        }

        return new T();
    }

    /// <summary>
    /// Handles the save and continue handlers.
    /// </summary>
    /// <param name="buttonAction">The name of the handler i.e. SaveAndContinue or SaveAndComeBackLater.</param>
    /// <param name="saveAndContinueRedirectUrl">The url to redirect to for the save and continue handler.</param>
    /// <param name="saveAndComeBackLaterRedirectUrl">The url to redirect to for the save and come back later handler.</param>
    /// <returns>A redirect result.</returns>
    protected RedirectResult ReturnSaveAndContinueRedirect(string buttonAction, string saveAndContinueRedirectUrl, string saveAndComeBackLaterRedirectUrl)
    {
        if (buttonAction == SaveAndContinueActionKey)
        {
            return Redirect(saveAndContinueRedirectUrl);
        }

        if (buttonAction == SaveAndComeBackLaterActionKey)
        {
            return Redirect(saveAndComeBackLaterRedirectUrl);
        }

        return Redirect("/Error");
    }

    /// <summary>
    /// Temporary method to set the back link.
    /// </summary>
    /// <remarks>Differs to the SetBackLink method by explicitly setting the previous page rather than using PreviousOrDefault.</remarks>
    /// <param name="previousPagePath">The path to the previous page.</param>
    /// <param name="currentPagePath">The path to the current page.</param>
    /// <returns>The completed page.</returns>
    protected async Task SetTempBackLink(string previousPagePath, string currentPagePath)
    {
        var session = await SessionManager.GetSessionAsync(HttpContext.Session) ?? new ReprocessorRegistrationSession();
        session.Journey = [previousPagePath, currentPagePath];
        SetBackLink(session, currentPagePath);

        await SaveSession(session, previousPagePath);
    }

    #endregion
}