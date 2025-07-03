using AutoMapper;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney;

public abstract class ExporterJourneyPageController<TController, TService, TDto, TViewModel> : BaseExporterController<TController>
    where TService : class
{
    protected readonly TService _service;

    protected ExporterJourneyPageController(
        ILogger<TController> logger,
        ISaveAndContinueService saveAndContinueService,
        ISessionManager<ExporterRegistrationSession> sessionManager,
        IMapper mapper,
        IConfiguration configuration,
        TService service)
        : base(logger, saveAndContinueService, sessionManager, mapper, configuration)
    {
        _service = service;
    }

    protected abstract string NextPageInJourney { get; }
    protected abstract string CurrentPageInJourney { get; }
    protected abstract string SaveAndContinueExporterPlaceholderKey { get; }
    protected abstract string CurrentPageViewLocation { get; }

    protected abstract Task<TDto> GetDtoAsync(Guid registrationId);
    protected abstract Task SaveDtoAsync(TDto dto);

    [HttpGet]
    public virtual async Task<IActionResult> Get(Guid? registrationId = null)
    {
        registrationId = await GetRegistrationIdAsync(registrationId);
        SetBackLink(CurrentPageInJourney);

        TViewModel vm = default;
        try
        {
            var dto = await GetDtoAsync(registrationId.Value);
            if (dto != null)
            {
                vm = Mapper.Map<TViewModel>(dto);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to retrieve data for registration {RegistrationId}", registrationId.Value);
        }
        finally
        {
            if (vm == null)
            {
                vm = Activator.CreateInstance<TViewModel>();
                // Set RegistrationId if needed
            }
        }

        return View(CurrentPageViewLocation, vm);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Post(TViewModel viewModel, string buttonAction)
    {
        if (!ModelState.IsValid)
        {
            return View(CurrentPageViewLocation, viewModel);
        }

        try
        {
            var dto = Mapper.Map<TDto>(viewModel);
            await SaveDtoAsync(dto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to save data");
            throw;
        }

        await PersistJourneyAndSession(CurrentPageInJourney, NextPageInJourney, SaveAndContinueAreas.ExporterRegistration, typeof(TController).Name,
            nameof(Get), JsonConvert.SerializeObject(viewModel), SaveAndContinueExporterPlaceholderKey);

        switch (buttonAction)
        {
            case SaveAndContinueActionKey:
                return Redirect(NextPageInJourney);
            case SaveAndComeBackLaterActionKey:
                return ApplicationSaved();
            default:
                return View(typeof(TController).Name);
        }
    }
}
