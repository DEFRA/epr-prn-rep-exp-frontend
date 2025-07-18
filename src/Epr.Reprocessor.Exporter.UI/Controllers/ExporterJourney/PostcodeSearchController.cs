using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Domain;
using Epr.Reprocessor.Exporter.UI.App.Services;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using EPR.Common.Authorization.Sessions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney
{
    public class PostcodeSearchController : BaseExporterController<PostcodeSearchController>
    {
        private readonly IValidationService _validationService;
        private readonly IPostcodeLookupService _postcodeLookupService;      
       
        private const string CurrentPageViewLocation = "~/Views/ExporterJourney/PostcodeSearch/PostcodeSearch.cshtml";
        private const string SelectAddressForServiceOfNoticesView = "~/Views/ExporterJourney/PostcodeSearch/SelectAddressForServiceOfNotices.cshtml";
        private const string ConfirmNoticesAddressView = "~/Views/ExporterJourney/PostcodeSearch/ConfirmNoticesAddress.cshtml";
        private const string NoAddressFoundView = "~/Views/ExporterJourney/PostcodeSearch/NoAddressFound.cshtml";

        public PostcodeSearchController(
                ILogger<PostcodeSearchController> logger,
                ISaveAndContinueService saveAndContinueService,
                ISessionManager<ExporterRegistrationSession> sessionManager,
                IMapper mapper,
                IPostcodeLookupService postcodeLookupService,
                IValidationService validationService
               ) : base(logger, saveAndContinueService, sessionManager, mapper)
        {
            _postcodeLookupService = postcodeLookupService;
            _validationService = validationService;
        }

        [HttpGet]
        [Route(PagePaths.ExporterPostcodeForNotices)]
        public async Task<IActionResult> Get()
        {
            await InitialiseSession();
            SetBackLink(PagePaths.ExporterPostcodeForNotices);

            var model = new AddressSearchViewModel();

            return View(CurrentPageViewLocation, model);
        }

        [HttpPost]
        [Route(PagePaths.ExporterPostcodeForServiceOfNotices)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExporterPostcodeForServiceOfNotices(AddressSearchViewModel model)
        {
            await InitialiseSession();
            SetBackLink(PagePaths.ExporterPostcodeForNotices);

            var validationResult = await _validationService.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                ModelState.AddValidationErrors(validationResult);
                return View(CurrentPageViewLocation, model);
            }

            var sessionLookupAddress = Session.RegistrationApplicationSession.ReprocessingSite!.ServiceOfNotice!.LookupAddress;
            sessionLookupAddress!.Postcode = model.Postcode;

            var addressList = await _postcodeLookupService.GetAddressListByPostcodeAsync(sessionLookupAddress!.Postcode);
            var newLookupAddress = new LookupAddress(model.Postcode, addressList, sessionLookupAddress.SelectedAddressIndex);
            Session.RegistrationApplicationSession.ReprocessingSite.ServiceOfNotice.LookupAddress = newLookupAddress;

            await SaveSession(CurrentPageViewLocation, PagePaths.PostcodeForServiceOfNotices);

            if (addressList is null || !addressList.Addresses.Any())
            {
                return RedirectToAction("NoAddressFound", new { addressLookupType = (int)AddressLookupType.LegalDocuments });
            }

            return Redirect(PagePaths.ExporterSelectAddressForServiceOfNotices);
        }
      
        [HttpGet]
        [Route(PagePaths.ExporterSelectAddressForServiceOfNotices)]
        public async Task<IActionResult> ExporterSelectAddressForServiceOfNotices(int? selectedIndex = null)
        {
            await InitialiseSession();
            SetBackLink(PagePaths.ExporterPostcodeForNotices);

            Session.RegistrationApplicationSession.ReprocessingSite!.ServiceOfNotice!.SetSourcePage(CurrentPageViewLocation);

            var lookupAddress = Session.RegistrationApplicationSession.ReprocessingSite!.ServiceOfNotice!.LookupAddress;

            //Postback: Address selected
            var addressSelected = selectedIndex.HasValue && selectedIndex > -1 && selectedIndex < lookupAddress!.AddressesForPostcode.Count;
            if (addressSelected)
            {
                lookupAddress.SelectedAddressIndex = selectedIndex;
                Session.RegistrationApplicationSession.ReprocessingSite!.ServiceOfNotice!.SetAddress(lookupAddress.SelectedAddress, AddressOptions.DifferentAddress);
            }

            await SaveSession(SelectAddressForServiceOfNoticesView, PagePaths.ExporterSelectAddressForServiceOfNotices);

            if (addressSelected)
            {
                return Redirect(PagePaths.ConfirmNoticesAddress);
            }

            var viewModel = new ViewModels.ExporterJourney.SelectAddressForServiceOfNoticesViewModel(lookupAddress);
            return View(SelectAddressForServiceOfNoticesView, viewModel);
        }

        [HttpGet(PagePaths.ExporterConfirmNoticesAddress)]
        public async Task<IActionResult> ConfirmNoticesAddress()
        {
            await InitialiseSession();
            SetBackLink(PagePaths.ExporterPostcodeForNotices);

            Session.RegistrationApplicationSession.ReprocessingSite?.ServiceOfNotice?.SetSourcePage(PagePaths
                .ConfirmNoticesAddress);

            await SaveSession(PagePaths.ExporterConfirmNoticesAddress, PagePaths.ExporterConfirmNoticesAddress);

            var lookupAddress = Session.RegistrationApplicationSession.ReprocessingSite!.ServiceOfNotice!.LookupAddress;

            var displayAddress = string.Empty;
            if (lookupAddress.SelectedAddress is not null)
            {
                var address = lookupAddress.SelectedAddress;
                displayAddress = string.Join(", ", new[] { address.AddressLine1, address.AddressLine2, address.Locality, address.Town, address.County, address.Postcode }
                                      .Where(addressPart => !string.IsNullOrWhiteSpace(addressPart)));
            }

            var viewModel = new ConfirmNoticesAddressViewModel
            {
                ConfirmAddress = displayAddress
            };

            return View(ConfirmNoticesAddressView, viewModel);
        }

        [HttpGet]
        [Route(PagePaths.ExporterNoAddressFound)]
        public async Task<IActionResult> NoAddressFound([FromQuery] AddressLookupType addressLookupType = AddressLookupType.ReprocessingSite)
        {
            await InitialiseSession();
            string previousPagePath;
            LookupAddress lookupAddress;

            switch (addressLookupType)
            {
                case AddressLookupType.LegalDocuments:
                    previousPagePath = PagePaths.PostcodeForServiceOfNotices;
                    lookupAddress = Session.RegistrationApplicationSession.ReprocessingSite!.ServiceOfNotice!.LookupAddress;
                    break;
                default:
                    previousPagePath = PagePaths.PostcodeOfReprocessingSite;
                    lookupAddress = Session.RegistrationApplicationSession.ReprocessingSite.LookupAddress;
                    break;
            }

            Session.Journey = new List<string> { previousPagePath, PagePaths.ExporterNoAddressFound };

            SetBackLink(PagePaths.ExporterPostcodeForNotices);

            await SaveSession(PagePaths.ExporterNoAddressFound);

            var model = new NoAddressFoundViewModel
            {
                Postcode = lookupAddress?.Postcode,
                LookupType = addressLookupType
            };

            return View(NoAddressFoundView, model);
        }
    }
}
