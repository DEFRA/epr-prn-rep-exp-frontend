using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney;

[ExcludeFromCodeCoverage]
public abstract class BaseExporterJourneyPageController<TController, TService, TDto, TViewModel>
    : BaseExporterController<TController>
    where TService : IBaseExporterService<TDto>
    where TViewModel : ExporterViewModelBase, new()
{
    protected readonly TService _service;

    protected readonly Dictionary<string, Func<TViewModel, IActionResult>> ButtonActionHandlers
        = new(StringComparer.OrdinalIgnoreCase);

    protected BaseExporterJourneyPageController(
        ILogger<TController> logger,
        ISaveAndContinueService saveAndContinueService,
        ISessionManager<ExporterRegistrationSession> sessionManager,
        IMapper mapper,
        IConfiguration configuration,
        TService service)
        : base(logger, saveAndContinueService, sessionManager, mapper, configuration)
    {
        _service = service;

        // Initialize button action handlers
        ButtonActionHandlers[SaveAndContinueActionKey] = vm => Redirect(NextPageInJourney);
        ButtonActionHandlers[SaveAndComeBackLaterActionKey] = vm => ApplicationSaved();
        ButtonActionHandlers[ConfirmAndContinueActionKey] = vm => Redirect(NextPageInJourney);
        ButtonActionHandlers[SaveAndContinueLaterActionKey] = vm => ApplicationSaved();
    }

    protected abstract string NextPageInJourney { get; }
    protected abstract string CurrentPageInJourney { get; }
    protected abstract string SaveAndContinueExporterPlaceholderKey { get; }
    protected abstract string CurrentPageViewLocation { get; }

    protected virtual Task<TDto> GetDtoAsync(Guid registrationId) => _service.GetByRegistrationId(registrationId);

    protected virtual Task SaveDtoAsync(TDto dto) => _service.Save(dto);

    [HttpGet]
    public virtual async Task<IActionResult> Get(Guid? registrationId = null)
    {
        registrationId = await GetRegistrationIdAsync(registrationId);
        SetBackLink(CurrentPageInJourney);

        TViewModel vm = default;
        try
        {
            // Retrieve the DTO based on the registration ID
            var dto = await GetDtoAsync(registrationId.Value);
            if (!object.Equals(dto, default(TDto)))
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
            // If the DTO is null, create a new ViewModel instance
            if (vm == null)
            {
                vm = new TViewModel();
                vm.RegistrationId = registrationId.Value;
            }
            else if (vm.RegistrationId == Guid.Empty)
            {
                vm.RegistrationId = registrationId.Value;
            }
        }

        return View(CurrentPageViewLocation, vm);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Post(TViewModel viewModel, string buttonAction)
    {
        // Validate the model state
        if (!ModelState.IsValid)
        {
            return View(CurrentPageViewLocation, viewModel);
        }

        // Reconstruct and save the DTO
        try
        {
            var dto = Mapper.Map<TDto>(viewModel);
            SaveDtoAsync(dto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unable to save data");
            throw;
        }

        // Record where we are in the journey
        await PersistJourneyAndSession(CurrentPageInJourney, NextPageInJourney, SaveAndContinueAreas.ExporterRegistration, typeof(TController).Name,
            nameof(Get), JsonConvert.SerializeObject(viewModel), SaveAndContinueExporterPlaceholderKey);

        // Handle button action
        if (ButtonActionHandlers.TryGetValue(buttonAction, out var handler))
            return handler(viewModel);

        return View(CurrentPageViewLocation, viewModel); 
    }
}
