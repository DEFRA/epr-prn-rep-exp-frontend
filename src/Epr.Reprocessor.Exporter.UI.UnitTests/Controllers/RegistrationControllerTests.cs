using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Controllers;
using Epr.Reprocessor.Exporter.UI.Enums;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration;
using Epr.Reprocessor.Exporter.UI.ViewModels.Reprocessor;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;
using EPR.Common.Authorization.Models;
using EPR.Common.Authorization.Sessions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;


namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers;

[TestClass]
public class RegistrationControllerTests
{
    private RegistrationController _controller;
    private Mock<ILogger<RegistrationController>> _logger;
    private Mock<UI.App.Services.Interfaces.ISaveAndContinueService> _userJourneySaveAndContinueService;
    private Mock<IValidationService> _validationService;
    private ReprocessorExporterRegistrationSession _session;
    private Mock<ISessionManager<ReprocessorExporterRegistrationSession>> _sessionManagerMock;
    private readonly Mock<HttpContext> _httpContextMock = new();
    private readonly Mock<ClaimsPrincipal> _userMock = new();
    private Mock<IStringLocalizer<RegistrationController>> _mockLocalizer = new();
    protected ITempDataDictionary TempDataDictionary;

    [TestInitialize]
    public void Setup()
    {
        // ResourcesPath should be 'Resources' but build environment differs from development environment
        // Work around = set ResourcesPath to non-existent location and test for resource keys rather than resource values
        var options = Options.Create(new LocalizationOptions { ResourcesPath = "Resources_not_found" });
        var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
        var localizer = new StringLocalizer<SelectAuthorisationType>(factory);

        _logger = new Mock<ILogger<RegistrationController>>();
        _userJourneySaveAndContinueService = new Mock<UI.App.Services.Interfaces.ISaveAndContinueService>();
        _sessionManagerMock = new Mock<ISessionManager<ReprocessorExporterRegistrationSession>>();
        _validationService = new Mock<IValidationService>();

        _controller = new RegistrationController(_logger.Object, _userJourneySaveAndContinueService.Object, _sessionManagerMock.Object, _validationService.Object, localizer);

        SetUpUserAndSessions();

        TempDataDictionary = new TempDataDictionary(this._httpContextMock.Object, new Mock<ITempDataProvider>().Object);
        _controller.TempData = TempDataDictionary;
    }


    [TestMethod]
    public async Task TaskList_ReturnsExpectedTaskListModel()
    {
        // Act
        var result = await _controller.TaskList() as ViewResult;

        // Assert
        Assert.IsNotNull(result, "Result should not be null");
        var model = result.Model as TaskListModel;
        Assert.IsNotNull(model, "Model should not be null");
        Assert.IsNotNull(model.TaskList, "TaskList should not be null");
        Assert.AreEqual(4, model.TaskList.Count, "TaskList should contain 4 items");

        // Verify each task item
        Assert.AreEqual("Site address and contact details", model.TaskList[0].TaskName);
        Assert.AreEqual("#", model.TaskList[0].TaskLink);
        Assert.AreEqual(TaskListStatus.NotStart, model.TaskList[0].status);

        Assert.AreEqual("Waste licenses, permits and exemptions", model.TaskList[1].TaskName);
        Assert.AreEqual("#", model.TaskList[1].TaskLink);
        Assert.AreEqual(TaskListStatus.CannotStartYet, model.TaskList[1].status);

        Assert.AreEqual("Reprocessing inputs and outputs", model.TaskList[2].TaskName);
        Assert.AreEqual("#", model.TaskList[2].TaskLink);
        Assert.AreEqual(TaskListStatus.CannotStartYet, model.TaskList[2].status);

        Assert.AreEqual("Sampling and inspection plan per material", model.TaskList[3].TaskName);
        Assert.AreEqual("#", model.TaskList[3].TaskLink);
        Assert.AreEqual(TaskListStatus.CannotStartYet, model.TaskList[3].status);
    }

    [TestMethod]
    public async Task WastePermitExemptions_ShouldReturnView()
    {
        _session = new ReprocessorExporterRegistrationSession();
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        // Act
        var result = await _controller.WastePermitExemptions();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }
    [TestMethod]
    public async Task WastePermitExemptions_Get_ReturnsViewWithModel()
    {
        // Arrange
        var session = new ReprocessorExporterRegistrationSession { Journey = new List<string>() };
        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // Act
        var result = await _controller.WastePermitExemptions();

        // Assert
        result.Should().BeOfType<ViewResult>();
        result.Should().NotBeNull();
    }
    [TestMethod]
    public async Task WastePermitExemptions_Post_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        var model = new WastePermitExemptionsViewModel(); 

        // Act
        var result = await _controller.WastePermitExemptions(model, "SaveAndContinue");

        // Assert
        result.Should().BeOfType<ViewResult>();
        result.Should().NotBeNull();
    }

    [TestMethod]
    public async Task AddressForNotices_ShouldReturnView()
    {
        _session = new ReprocessorExporterRegistrationSession();
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        // Act
        var result = await _controller.AddressForNotices();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }
    [TestMethod]
    public async Task AddressForNotices_ShouldSetBackLink()
    {
        // Act
        var result = await _controller.AddressForNotices() as ViewResult;
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;
        // Assert
        result.Should().BeOfType<ViewResult>();
        backlink.Should().Be(PagePaths.AddressForLegalDocuments);
    }
    [TestMethod]
    public async Task AddressForNotices_Get_ReturnsViewWithModel()
    {
        // Arrange
        var session = new ReprocessorExporterRegistrationSession { Journey = new List<string>() };
        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // Act
        var result = await _controller.AddressForNotices();

        // Assert
        result.Should().BeOfType<ViewResult>();
        result.Should().NotBeNull();
    }
    [TestMethod]
    public async Task AddressForNotices_Post_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        var model = new AddressForNoticesViewModel();
        _controller.ModelState.AddModelError("SiteLocationId", "Required");

        // Act
        var result = await _controller.AddressForNotices(model, "SaveAndContinue");

        // Assert
        result.Should().BeOfType<ViewResult>();
        result.Should().NotBeNull();
    }
    [TestMethod]
    public async Task AddressForNotices_ShouldSaveSession()
    {
        _session = new ReprocessorExporterRegistrationSession();
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        // Act
        var result = await _controller.AddressForNotices() as ViewResult;
        var session = _controller.HttpContext.Session as ReprocessorExporterRegistrationSession;
        // Assert
        result.Should().BeOfType<ViewResult>();

        _sessionManagerMock.Verify(x => x.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<ReprocessorExporterRegistrationSession>()), Times.Once);

        _session.Journey.Count.Should().Be(1);
        _session.Journey[0].Should().Be(PagePaths.AddressForNotices);
    }


    [TestMethod]
    public async Task UkSiteLocation_ShouldReturnView()
    {
        _session = new ReprocessorExporterRegistrationSession();
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        // Act
        var result = await _controller.UKSiteLocation();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [TestMethod]
    public async Task UkSiteLocation_ShouldSetBackLink()
    {
        // Act
        var result = await _controller.UKSiteLocation() as ViewResult;
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;
        // Assert
        result.Should().BeOfType<ViewResult>();
        backlink.Should().Be(PagePaths.AddressForLegalDocuments);
    }

    [TestMethod]
    public async Task UKSiteLocation_Get_ReturnsViewWithModel()
    {
        // Arrange
        var session = new ReprocessorExporterRegistrationSession { Journey = new List<string>() };
        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // Act
        var result = await _controller.UKSiteLocation();

        // Assert
        result.Should().BeOfType<ViewResult>();
        result.Should().NotBeNull();
    }

    [TestMethod]
    public async Task UKSiteLocation_Post_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        var model = new UKSiteLocationViewModel();
        _controller.ModelState.AddModelError("SiteLocationId", "Required");

        // Act
        var result = await _controller.UKSiteLocation(model, "SaveAndContinue");

        // Assert
        result.Should().BeOfType<ViewResult>();
        result.Should().NotBeNull();
    }

    [TestMethod]
    public async Task UkSiteLocation_ShouldSaveSession()
    {
        _session = new ReprocessorExporterRegistrationSession();
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        // Act
        var result = await _controller.UKSiteLocation() as ViewResult;
        var session = _controller.HttpContext.Session as ReprocessorExporterRegistrationSession;
        // Assert
        result.Should().BeOfType<ViewResult>();

        _sessionManagerMock.Verify(x => x.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<ReprocessorExporterRegistrationSession>()), Times.Once);

        _session.Journey.Count.Should().Be(2);
        _session.Journey[0].Should().Be(PagePaths.AddressForLegalDocuments);
        _session.Journey[1].Should().Be(PagePaths.CountryOfReprocessingSite);
    }

    [TestMethod]
    public async Task UkSiteLocation_ShouldSetFromSaveAndContinue()
    {
        var expetcedModel = new UKSiteLocationViewModel() { SiteLocationId = Enums.UkNation.England };
        _session = new ReprocessorExporterRegistrationSession();
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        _userJourneySaveAndContinueService.Setup(x => x.GetLatestAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new SaveAndContinueResponseDto
        {
            Action = nameof(RegistrationController.UKSiteLocation),
            Controller = nameof(RegistrationController),
            Area = SaveAndContinueAreas.Registration,
            CreatedOn = DateTime.UtcNow,
            Id = 1,
            RegistrationId = 1,
            Parameters = JsonConvert.SerializeObject(expetcedModel)
        });

        // Act
        var result = await _controller.UKSiteLocation() as ViewResult;
        var session = _controller.HttpContext.Session as ReprocessorExporterRegistrationSession;
        var model = result.Model as UKSiteLocationViewModel;

        // Assert
        result.Should().BeOfType<ViewResult>();
        _sessionManagerMock.Verify(x => x.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<ReprocessorExporterRegistrationSession>()), Times.Once);

        model.Should().BeEquivalentTo(expetcedModel);
    }

    [TestMethod]
    public async Task UkSiteLocation_ShouldSetStubTempDataSaveAndContinue()
    {
        var expetcedModel = new UKSiteLocationViewModel() { SiteLocationId = Enums.UkNation.England };
        _session = new ReprocessorExporterRegistrationSession();
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        _controller.TempData["SaveAndContinueUkSiteNationKey"] = JsonConvert.SerializeObject(expetcedModel);

        _userJourneySaveAndContinueService.Setup(x => x.GetLatestAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((SaveAndContinueResponseDto)null);

        // Act
        var result = await _controller.UKSiteLocation() as ViewResult;
        var session = _controller.HttpContext.Session as ReprocessorExporterRegistrationSession;
        var model = result.Model as UKSiteLocationViewModel;

        // Assert
        result.Should().BeOfType<ViewResult>();
        _sessionManagerMock.Verify(x => x.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<ReprocessorExporterRegistrationSession>()), Times.Once);

        model.Should().BeEquivalentTo(expetcedModel);
    }

    [TestMethod]
    public async Task UkSiteLocation_OnSubmit_ShouldValidateModel()
    {
        var saveAndContinue = "SaveAndContinue";
        var model = new UKSiteLocationViewModel() { SiteLocationId = null };
        var expectedErrorMessage = "Select the country the reprocessing site is located in.";
        ValidateViewModel(model);

        // Act
        var result = await _controller.UKSiteLocation(model, saveAndContinue);
        var modelState = _controller.ModelState;

        // Assert
        result.Should().BeOfType<ViewResult>();

        Assert.IsTrue(modelState["SiteLocationId"].Errors.Count == 1);
        Assert.AreEqual(expectedErrorMessage, modelState["SiteLocationId"].Errors[0].ErrorMessage);
    }

    [TestMethod]
    public async Task UkSiteLocation_OnSubmit_SaveAndContinue_ShouldRedirectNextPage()
    {
        var saveAndContinue = "SaveAndContinue";
        var model = new UKSiteLocationViewModel() { SiteLocationId = Enums.UkNation.England };

        ValidateViewModel(model);

        // Act
        var result = await _controller.UKSiteLocation(model, saveAndContinue) as RedirectResult;

        // Assert
        result.Should().BeOfType<RedirectResult>();

        result.Url.Should().Be(PagePaths.PostcodeOfReprocessingSite);
    }

    [TestMethod]
    public async Task UkSiteLocation_OnSubmit_SaveAndContinue_ShouldSetBackLink()
    {
        var saveAndContinue = "SaveAndContinue";
        _session = new ReprocessorExporterRegistrationSession() { Journey = new List<string> { PagePaths.AddressForLegalDocuments, PagePaths.CountryOfReprocessingSite } };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);
        var model = new UKSiteLocationViewModel() { SiteLocationId = Enums.UkNation.England };

        ValidateViewModel(model);

        // Act
        var result = await _controller.UKSiteLocation(model, saveAndContinue) as RedirectResult;
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;
        // Assert
        result.Should().BeOfType<RedirectResult>();

        backlink.Should().Be(PagePaths.AddressForLegalDocuments);
    }

    [TestMethod]
    public async Task UkSiteLocation_OnSubmit_SaveAndComeBackLater_ShouldRedirectNextPage()
    {
        var saveAndComeBackLater = "SaveAndComeBackLater";
        var model = new UKSiteLocationViewModel() { SiteLocationId = Enums.UkNation.England };
        var expectedModel = JsonConvert.SerializeObject(model);

        // Act
        var result = await _controller.UKSiteLocation(model, saveAndComeBackLater) as RedirectResult;

        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Url.Should().Be(PagePaths.ApplicationSaved);
    }

    [TestMethod]
    public async Task UkSiteLocation_OnSubmit_SaveAndComeBackLater_ShouldSetBackLink()
    {
        var saveAndComeBackLater = "SaveAndComeBackLater";
        _session = new ReprocessorExporterRegistrationSession() { Journey = new List<string> { PagePaths.AddressForLegalDocuments, PagePaths.CountryOfReprocessingSite } };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        var model = new UKSiteLocationViewModel() { SiteLocationId = Enums.UkNation.England };
        var expectedModel = JsonConvert.SerializeObject(model);

        // Act
        var result = await _controller.UKSiteLocation(model, saveAndComeBackLater) as RedirectResult;
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;
        // Assert
        result.Should().BeOfType<RedirectResult>();
        backlink.Should().Be(PagePaths.AddressForLegalDocuments);
    }

    [TestMethod]
    public async Task NoAddressFound_ShouldReturnViewWithModel()
    {
        var result = await _controller.NoAddressFound() as ViewResult;
        var model = result.Model as NoAddressFoundViewModel;

        result.Should().BeOfType<ViewResult>();
        model.Should().NotBeNull();
        model.Postcode.Should().Be("[TEST POSTCODE REPLACE WITH SESSION]");
    }

    [TestMethod]
    public async Task PostcodeOfReprocessingSite_Get_ShouldReturnViewWithModel()
    {
        var result = await _controller.PostcodeOfReprocessingSite() as ViewResult;
        var model = result.Model as PostcodeOfReprocessingSiteViewModel;

        result.Should().BeOfType<ViewResult>();
        model.Should().NotBeNull();
    }

    [TestMethod]
    public async Task PostcodeOfReprocessingSite_Post_ShouldReturnViewWithModel()
    {
        var model = new PostcodeOfReprocessingSiteViewModel();
        var result = await _controller.PostcodeOfReprocessingSite(model) as ViewResult;

        result.Should().BeOfType<ViewResult>();
        result.Model.Should().Be(model);
    }

    [TestMethod]
    public async Task AddressOfReprocessingSite_Get_ShouldReturnViewWithModel()
    {
        // Act
        var result = await _controller.AddressOfReprocessingSite() as ViewResult;
        var model = result.Model as AddressOfReprocessingSiteViewModel;

        // Assert
        result.Should().BeOfType<ViewResult>();
        model.Should().NotBeNull();
        model.BusinessAddress.Should().NotBeNull();
        model.RegisteredAddress.Should().BeNull();
    }

    [TestMethod]
    public async Task AddressOfReprocessingSite_Post_ValidModel_ShouldReturnRedirectResult()
    {
        // Arrange
        var model = new AddressOfReprocessingSiteViewModel
        {
            SelectedOption = Enums.AddressOptions.SiteAddress,
            BusinessAddress = new UI.ViewModels.Shared.AddressViewModel
            {
                AddressLine1 = "Address line 1",
                County = "Greater Glasgow",
                TownOrCity = "Glasgow",
                Postcode = "G5 0US"
            },
            RegisteredAddress = null,
        };

        _validationService.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // Act
        var result = await _controller.AddressOfReprocessingSite(model) as RedirectResult;

        // Assert
        result.Should().NotBeNull();
        result.Url.Should().Be(PagePaths.CountryOfReprocessingSite);
    }

    [TestMethod]
    public async Task AddressOfReprocessingSite_Post_InvalidModel_ShouldReturnViewWithDefaultModel()
    {
        // Arrange
        var model = new AddressOfReprocessingSiteViewModel();

        var validationResult = new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure>
            {
                new()
                {
                     PropertyName = "SelectedOption",
                     ErrorMessage = "SelectedOption is required",
                }
            });

        _validationService.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.AddressOfReprocessingSite(model) as ViewResult;
        var returnedModel = result.Model as AddressOfReprocessingSiteViewModel;

        // Assert
        result.Should().BeOfType<ViewResult>();
        returnedModel.Should().NotBeNull();
    }

    [TestMethod]
    public async Task AddressOfReprocessingSite_Post_InvalidModel_ShouldPreserveModelStateErrors()
    {
        // Arrange
        var model = new AddressOfReprocessingSiteViewModel();
        var validationResult = new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure>
            {
                new()
                {
                     PropertyName = "SelectedOption",
                     ErrorMessage = "SelectedOption is required",
                }
            });

        _validationService.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.AddressOfReprocessingSite(model) as ViewResult;

        // Assert
        result.Should().BeOfType<ViewResult>();
        _controller.ModelState.ErrorCount.Should().Be(1);
    }

    [TestMethod]
    public async Task ProvideSiteGridReference_ShouldReturnView()
    {
        _session = new ReprocessorExporterRegistrationSession();
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        // Act
        var result = await _controller.ProvideSiteGridReference();
        var model = (result as ViewResult).Model;

        // Assert
        result.Should().BeOfType<ViewResult>();
        model.Should().BeOfType<ProvideSiteGridReferenceViewModel>();
    }

    [TestMethod]
    public async Task ProvideSiteGridReference_ShouldSetBackLink()
    {
        // Act
        var result = await _controller.ProvideSiteGridReference() as ViewResult;
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;

        // Assert
        result.Should().BeOfType<ViewResult>();

        backlink.Should().Be(PagePaths.AddressOfReprocessingSite);
    }

    [TestMethod]
    [DataRow(null, "Enter the site’s grid reference")]
    [DataRow("T%%", "Grid references must include numbers")]
    [DataRow("TF333", "Enter a grid reference with at least 4 numbers")]
    [DataRow("TF32141934322332", "Enter a grid reference with no more than 10 numbers")]
    public async Task ProvideSiteGridReference_OnSubmit_ValidateGridReference_ShouldValidateModel(string gridReference, string expectedErrorMessage)
    {
        var saveAndContinue = "SaveAndContinue";
        var model = new ProvideSiteGridReferenceViewModel() { GridReference = gridReference };
        ValidateViewModel(model);

        // Act
        var result = await _controller.ProvideSiteGridReference(model, saveAndContinue);
        var modelState = _controller.ModelState;

        // Assert
        result.Should().BeOfType<ViewResult>();

        var modelStateErrorCount = modelState.ContainsKey("GridReference") ? modelState["GridReference"].Errors.Count : modelState[""].Errors.Count;
        var modelStateErrorMessage = modelState.ContainsKey("GridReference") ? modelState["GridReference"].Errors[0].ErrorMessage : modelState[""].Errors[0].ErrorMessage;

        Assert.AreEqual(1, modelStateErrorCount);
        Assert.AreEqual(expectedErrorMessage, modelStateErrorMessage);
    }

    [TestMethod]
    [DataRow("TF123434")]
    [DataRow("TF3333")]
    [DataRow("TF3214193478")]
    public async Task ProvideSiteGridReference_OnSubmit_ShouldBeSuccessful(string gridReference)
    {
        var saveAndContinue = "SaveAndContinue";
        var model = new ProvideSiteGridReferenceViewModel() { GridReference = gridReference };
        ValidateViewModel(model);

        // Act
        var result = await _controller.ProvideSiteGridReference(model, saveAndContinue);
        var modelState = _controller.ModelState;

        // Assert
        result.Should().BeOfType<RedirectResult>();
    }

    [TestMethod]
    [DataRow("SaveAndContinue", PagePaths.AddressOfReprocessingSite)]
    [DataRow("SaveAndComeBackLater", PagePaths.AddressOfReprocessingSite)]
    public async Task ProvideSiteGridReference_OnSubmit_ShouldSetBackLink(string actionButton, string backLinkUrl)
    {
        _session = new ReprocessorExporterRegistrationSession() { Journey = new List<string> { PagePaths.AddressOfReprocessingSite, PagePaths.GridReferenceForEnteredReprocessingSite } };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        var model = new ProvideSiteGridReferenceViewModel() { GridReference = "1245412545" };
        var expectedModel = JsonConvert.SerializeObject(model);

        // Act
        await _controller.ProvideSiteGridReference(model, actionButton);
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;
        // Assert

        backlink.Should().Be(backLinkUrl);
    }

    [TestMethod]
    [DataRow("SaveAndContinue", "/")]
    [DataRow("SaveAndComeBackLater", PagePaths.ApplicationSaved)]
    public async Task ProvideSiteGridReference_OnSubmit_ShouldRedirect(string actionButton, string expectedReturnUrl)
    {
        _session = new ReprocessorExporterRegistrationSession() { Journey = new List<string> { "/", PagePaths.GridReferenceForEnteredReprocessingSite } };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        var model = new ProvideSiteGridReferenceViewModel() { GridReference = "1245412545" };
        var expectedModel = JsonConvert.SerializeObject(model);

        // Act
        var result = await _controller.ProvideSiteGridReference(model, actionButton) as RedirectResult;
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;
        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Url.Should().Be(expectedReturnUrl);
    }

    [TestMethod]
    public async Task ManualAddressForServiceOfNotices_Get_ReturnsViewWithModel()
    {
        // Arrange
        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorExporterRegistrationSession());

        // Act
        var result = await _controller.ManualAddressForServiceOfNotices();
        var viewResult = result as ViewResult;

        // Assert
        using (new AssertionScope())
        {
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Be("ManualAddressForServiceOfNotices");
            viewResult.Model.Should().BeOfType<ManualAddressForServiceOfNoticesViewModel>();
        }
    }


    [TestMethod]
    public async Task ManualAddressForServiceOfNotices_Post_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        var model = new ManualAddressForServiceOfNoticesViewModel();
        var validationResult = new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure>
            {
                new()
                {
                     PropertyName = "Test",
                     ErrorMessage = "Test",
                }
            });

        _validationService.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.ManualAddressForServiceOfNotices(model, "SaveAndContinue");
        var viewResult = result as ViewResult;

        // Assert
        using (new AssertionScope())
        {
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().Be(model);
        }
    }

    [TestMethod]
    public async Task ManualAddressForServiceOfNotices_Post_SaveAndContinue_RedirectsCorrectly()
    {
        // Arrange
        var model = new ManualAddressForServiceOfNoticesViewModel();
        _validationService.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorExporterRegistrationSession());

        // Act
        var result = await _controller.ManualAddressForServiceOfNotices(model, "SaveAndContinue");
        var redirectResult = result as RedirectResult;

        // Assert
        using (new AssertionScope())
        {
            redirectResult.Should().NotBeNull();
            redirectResult.Url.Should().Be(PagePaths.RegistrationLanding);
        }
    }

    [TestMethod]
    public async Task ManualAddressForServiceOfNotices_Post_SaveAndComeBackLater_RedirectsCorrectly()
    {
        // Arrange
        var model = new ManualAddressForServiceOfNoticesViewModel();
        _validationService.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorExporterRegistrationSession());

        // Act
        var result = await _controller.ManualAddressForServiceOfNotices(model, "SaveAndComeBackLater");
        var redirectResult = result as RedirectResult;

        // Assert
        using (new AssertionScope())
        {
            redirectResult.Should().NotBeNull();
            redirectResult.Url.Should().Be(PagePaths.ApplicationSaved);
        }
    }

    [TestMethod]
    public async Task ProvideGridReferenceOfReprocessingSite_ShouldReturnView()
    {
        _session = new ReprocessorExporterRegistrationSession();
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        // Act
        var result = await _controller.ProvideGridReferenceOfReprocessingSite();
        var model = (result as ViewResult).Model;

        // Assert
        result.Should().BeOfType<ViewResult>();
        model.Should().BeOfType<ProvideGridReferenceOfReprocessingSiteViewModel>();
    }

    [TestMethod]
    public async Task ProvideGridReferenceOfReprocessingSite_ShouldSetBackLink()
    {
        // Act
        var result = await _controller.ProvideGridReferenceOfReprocessingSite() as ViewResult;
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;

        // Assert
        result.Should().BeOfType<ViewResult>();

        backlink.Should().Be(PagePaths.CountryOfReprocessingSite);
    }

    [TestMethod]
    [DataRow(null, "Enter the site’s grid reference")]
    [DataRow("T$", "Grid references must include numbers")]
    [DataRow("T343", "Enter a grid reference with at least 4 numbers")]
    [DataRow("TF234323456782", "Enter a grid reference with no more than 10 numbers")]
    public async Task ProvideGridReferenceOfReprocessingSite_OnSubmit_ValidateGridReference_ShouldValidateModel(string gridReference, string expectedErrorMessage)
    {
        var saveAndContinue = "SaveAndContinue";
        var model = new ProvideGridReferenceOfReprocessingSiteViewModel() { GridReference = gridReference };
        ValidateViewModel(model);

        // Act
        var result = await _controller.ProvideGridReferenceOfReprocessingSite(model, saveAndContinue);
        var modelState = _controller.ModelState;

        // Assert
        result.Should().BeOfType<ViewResult>();

        var modelStateErrorCount = modelState.ContainsKey("GridReference") ? modelState["GridReference"].Errors.Count : modelState[""].Errors.Count;
        var modelStateErrorMessage = modelState.ContainsKey("GridReference") ? modelState["GridReference"].Errors[0].ErrorMessage : modelState[""].Errors[0].ErrorMessage;

        Assert.AreEqual(1, modelStateErrorCount);
        Assert.AreEqual(expectedErrorMessage, modelStateErrorMessage);
    }

    [TestMethod]
    [DataRow("SaveAndContinue", PagePaths.CountryOfReprocessingSite)]
    [DataRow("SaveAndComeBackLater", PagePaths.CountryOfReprocessingSite)]
    public async Task ProvideGridReferenceOfReprocessingSite_OnSubmit_ShouldSetBackLink(string actionButton, string backLinkUrl)
    {
        _session = new ReprocessorExporterRegistrationSession() { Journey = new List<string> { PagePaths.CountryOfReprocessingSite, PagePaths.GridReferenceOfReprocessingSite } };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        var model = new ProvideGridReferenceOfReprocessingSiteViewModel() { GridReference = "TS1245412545" };
        var expectedModel = JsonConvert.SerializeObject(model);

        // Act
        await _controller.ProvideGridReferenceOfReprocessingSite(model, actionButton);
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;
        // Assert

        backlink.Should().Be(backLinkUrl);
    }

    [TestMethod]
    [DataRow("SaveAndContinue", "/")]
    [DataRow("SaveAndComeBackLater", PagePaths.ApplicationSaved)]
    public async Task ProvideGridReferenceOfReprocessingSite_OnSubmit_ShouldRedirect(string actionButton, string expectedReturnUrl)
    {
        _session = new ReprocessorExporterRegistrationSession() { Journey = new List<string> { "/", PagePaths.GridReferenceOfReprocessingSite } };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        var model = new ProvideGridReferenceOfReprocessingSiteViewModel() { GridReference = "1245412545" };
        var expectedModel = JsonConvert.SerializeObject(model);

        // Act
        var result = await _controller.ProvideGridReferenceOfReprocessingSite(model, actionButton) as RedirectResult;
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;
        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Url.Should().Be(expectedReturnUrl);
    }

    [TestMethod]
    public async Task SelectAddressForServiceOfNotices_Get_ReturnsViewWithModel()
    {
        // Arrange
        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorExporterRegistrationSession());

        // Act
        var result = await _controller.SelectAddressForServiceOfNotices();
        var viewResult = result as ViewResult;

        // Assert
        using (new AssertionScope())
        {
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Be("SelectAddressForServiceOfNotices");
            viewResult.Model.Should().BeOfType<SelectAddressForServiceOfNoticesViewModel>();
        }
    }

    [TestMethod]
    public async Task SelectedAddressForServiceOfNotices_Get_SaveAndContinue_RedirectsCorrectly()
    {
        // Arrange
        var model = new SelectedAddressViewModel
        {
            SelectedIndex = 0,
            Postcode = "G5 0US"
        };

        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorExporterRegistrationSession());

        _validationService.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        // Act
        var result = await _controller.SelectedAddressForServiceOfNotices(model);
        var redirectResult = result as RedirectResult;

        // Assert
        using (new AssertionScope())
        {
            redirectResult.Should().NotBeNull();
            redirectResult.Url.Should().Be(PagePaths.RegistrationLanding);
        }
    }


    [TestMethod]
    public async Task ManualAddressForReprocessingSite_Get_ReturnsViewWithModel()
    {
        // Arrange
        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorExporterRegistrationSession());

        // Act
        var result = await _controller.ManualAddressForReprocessingSite();
        var viewResult = result as ViewResult;

        // Assert
        using (new AssertionScope())
        {
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Be("ManualAddressForReprocessingSite");
            viewResult.Model.Should().BeOfType<ManualAddressForReprocessingSiteViewModel>();
        }
    }


    [TestMethod]
    public async Task ManualAddressForReprocessingSite_Post_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        var model = new ManualAddressForReprocessingSiteViewModel();
        var validationResult = new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure>
            {
                new()
                {
                     PropertyName = "Test",
                     ErrorMessage = "Test",
                }
            });

        _validationService.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.ManualAddressForReprocessingSite(model, "SaveAndContinue");
        var viewResult = result as ViewResult;

        // Assert
        using (new AssertionScope())
        {
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().Be(model);
        }
    }

    [TestMethod]
    public async Task ManualAddressForReprocessingSite_Post_SaveAndContinue_RedirectsCorrectly()
    {
        // Arrange
        var model = new ManualAddressForReprocessingSiteViewModel();
        _validationService.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorExporterRegistrationSession());

        // Act
        var result = await _controller.ManualAddressForReprocessingSite(model, "SaveAndContinue");
        var redirectResult = result as RedirectResult;

        // Assert
        using (new AssertionScope())
        {
            redirectResult.Should().NotBeNull();
            redirectResult.Url.Should().Be(PagePaths.AddressForNotices);
        }
    }

    [TestMethod]
    public async Task ManualAddressForReprocessingSite_Post_SaveAndComeBackLater_RedirectsCorrectly()
    {
        // Arrange
        var model = new ManualAddressForReprocessingSiteViewModel();
        _validationService.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorExporterRegistrationSession());

        // Act
        var result = await _controller.ManualAddressForReprocessingSite(model, "SaveAndComeBackLater");
        var redirectResult = result as RedirectResult;

        // Assert
        using (new AssertionScope())
        {
            redirectResult.Should().NotBeNull();
            redirectResult.Url.Should().Be(PagePaths.ApplicationSaved);
        }
    }


    [TestMethod]
    public async Task PostcodeForServiceOfNotices_Get_ReturnsViewWithModel()
    {
        // Arrange
        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorExporterRegistrationSession());

        // Act
        var result = await _controller.PostcodeForServiceOfNotices();
        var viewResult = result as ViewResult;

        // Assert
        using (new AssertionScope())
        {
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Be("PostcodeForServiceOfNotices");
            viewResult.Model.Should().BeOfType<PostcodeForServiceOfNoticesViewModel>();
        }
    }


    [TestMethod]
    public async Task PostcodeForServiceOfNotices_Post_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        var model = new PostcodeForServiceOfNoticesViewModel();
        var validationResult = new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure>
            {
                new()
                {
                     PropertyName = "Test",
                     ErrorMessage = "Test",
                }
            });

        _validationService.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.PostcodeForServiceOfNotices(model, "SaveAndContinue");
        var viewResult = result as ViewResult;

        // Assert
        using (new AssertionScope())
        {
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().Be(model);
        }
    }

    [TestMethod]
    public async Task PostcodeForServiceOfNotices_Post_SaveAndContinue_RedirectsCorrectly()
    {
        // Arrange
        var model = new PostcodeForServiceOfNoticesViewModel();
        _validationService.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorExporterRegistrationSession());

        // Act
        var result = await _controller.PostcodeForServiceOfNotices(model, "SaveAndContinue");
        var redirectResult = result as RedirectResult;

        // Assert
        using (new AssertionScope())
        {
            redirectResult.Should().NotBeNull();
            redirectResult.Url.Should().Be(PagePaths.SelectAddressForServiceOfNotices);
        }
    }

    [TestMethod]
    public async Task PostcodeForServiceOfNotices_Post_SaveAndComeBackLater_RedirectsCorrectly()
    {
        // Arrange
        var model = new PostcodeForServiceOfNoticesViewModel();
        _validationService.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorExporterRegistrationSession());

        // Act
        var result = await _controller.PostcodeForServiceOfNotices(model, "SaveAndComeBackLater");
        var redirectResult = result as RedirectResult;

        // Assert
        using (new AssertionScope())
        {
            redirectResult.Should().NotBeNull();
            redirectResult.Url.Should().Be(PagePaths.SelectAddressForServiceOfNotices);
        }
    }


    [TestMethod]
    public async Task SelectAddressForReprocessingSite_Get_ReturnsViewWithModel()
    {
        // Arrange
        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorExporterRegistrationSession());

        // Act
        var result = await _controller.SelectAddressForReprocessingSite();
        var viewResult = result as ViewResult;

        // Assert
        using (new AssertionScope())
        {
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Be("SelectAddressForReprocessingSite");
            viewResult.Model.Should().BeOfType<SelectAddressForReprocessingSiteViewModel>();
        }
    }

    [TestMethod]
    public async Task SelectedAddressForReprocessingSite_Get_SaveAndContinue_RedirectsCorrectly()
    {
        // Arrange
        var model = new SelectedAddressViewModel
        {
            SelectedIndex = 0,
            Postcode = "G5 0US"
        };

        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorExporterRegistrationSession());

        _validationService.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        // Act
        var result = await _controller.SelectedAddressForReprocessingSite(model);
        var redirectResult = result as RedirectResult;

        // Assert
        using (new AssertionScope())
        {
            redirectResult.Should().NotBeNull();
            redirectResult.Url.Should().Be(PagePaths.GridReferenceOfReprocessingSite);
        }
    }

    [TestMethod]
    public async Task ApplicationSaved_ReturnsExpectedViewResult()
    {
        // Act
        var result = _controller.ApplicationSaved();

        // Assert
        Assert.AreSame(typeof(ViewResult), result.GetType(), "Result should be of type ViewResult");

    }

    [TestMethod]
    public async Task ConfirmNoticesAddress_ReturnsExpectedViewResult()
    {
        // Act
        var result = _controller.ConfirmNoticesAddress();
        var viewResult = result as ViewResult;
        // Assert
        using (new AssertionScope())
        {
            Assert.AreSame(typeof(ViewResult), result.GetType(), "Result should be of type ViewResult");
            viewResult.Model.Should().BeOfType<ConfirmNoticesAddressViewModel>();
        }
    }

    [TestMethod]
    public async Task ConfirmNoticesAddress_Sets_BackLink_ReturnsExpectedViewResult()
    {
        // Act
        var result = _controller.ConfirmNoticesAddress();
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;

        // Assert
        Assert.AreEqual(backlink, PagePaths.SelectAddressForServiceOfNotices);
    }

    [TestMethod]
    public async Task ConfirmNoticesAddress_OnSubmit_ReturnsExpectedViewResult()
    {
        var model = new ConfirmNoticesAddressViewModel();
        // Act
        var result = _controller.ConfirmNoticesAddress(model);
        var viewResult = result as ViewResult;
        // Assert
        using (new AssertionScope())
        {
            Assert.AreSame(typeof(ViewResult), result.GetType(), "Result should be of type ViewResult");
        }
    }

    [TestMethod]
    public async Task ConfirmNoticesAddress_OnSubmit_Sets_BackLink_ReturnsExpectedViewResult()
    {
        var model = new ConfirmNoticesAddressViewModel();
        // Act
        var result = _controller.ConfirmNoticesAddress(model);
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;

        // Assert
        Assert.AreEqual(backlink, PagePaths.SelectAddressForServiceOfNotices);
    }


    [TestMethod]
    public async Task CheckAnswers_Get_ReturnsViewWithModel()
    {
        // Arrange
        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorExporterRegistrationSession());

        // Act
        var result = await _controller.CheckAnswers();
        var viewResult = result as ViewResult;

        // Assert
        using (new AssertionScope())
        {
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().BeOfType<CheckAnswersViewModel>();
        }
    }

    [TestMethod]
    public async Task CheckAnswers_Post_SaveAndContinue_RedirectsCorrectly()
    {
        // Arrange
        var model = new CheckAnswersViewModel();

        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorExporterRegistrationSession());

        // Act
        var result = await _controller.CheckAnswers(model);
        var redirectResult = result as RedirectResult;

        // Assert
        using (new AssertionScope())
        {
            redirectResult.Should().NotBeNull();
            redirectResult.Url.Should().Be(PagePaths.RegistrationLanding);
        }
    }

    [TestMethod]
    public async Task SelectAuthorisationType_ReturnsExpectedViewResult()
    {
        // Act
        var result = _controller.SelectAuthorisationType();
        var viewResult = result as ViewResult;

        // Assert
        using (new AssertionScope())
        {
            Assert.AreSame(typeof(ViewResult), result.GetType(), "Result should be of type ViewResult");
            viewResult.Model.Should().BeOfType<SelectAuthorisationTypeViewModel>();
        }
    }

    [TestMethod]
    [DataRow("GB-ENG", 4)]
    [DataRow("GB-WLS", 4)]
    [DataRow("GB-SCT", 3)]
    [DataRow("GB-NIR", 3)]
    public async Task SelectAuthorisationType_ByNationCode_ReturnsExpectedViewResult(string nationCode, int expectedResult)
    {
        // Act
        var result = _controller.SelectAuthorisationType(nationCode);
        var viewResult = result as ViewResult;

        // Assert
        using (new AssertionScope())
        {
            Assert.AreSame(typeof(ViewResult), result.GetType(), "Result should be of type ViewResult");
            (viewResult.Model as SelectAuthorisationTypeViewModel).AuthorisationTypes.Count.Should().Be(expectedResult);
        }
    }

    [TestMethod]
    public async Task SelectAuthorisationType_SetsBackLink_ReturnsExpectedViewResult()
    {
        // Act
        var result = _controller.SelectAuthorisationType();
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;

        // Assert
        using (new AssertionScope())
        {
            Assert.AreSame(typeof(ViewResult), result.GetType(), "Result should be of type ViewResult");
            backlink.Should().Be(PagePaths.RegistrationLanding);
        }
    }

    [TestMethod]
    [DataRow("SaveAndContinue", PagePaths.RegistrationLanding)]
    [DataRow("SaveAndComeBackLater", PagePaths.RegistrationLanding)]
    public async Task SelectAuthorisationType_OnSubmit_ShouldSetBackLink(string actionButton, string backLinkUrl)
    {
        //Arrange
        _session = new ReprocessorExporterRegistrationSession() { Journey = new List<string> { PagePaths.CountryOfReprocessingSite, PagePaths.GridReferenceOfReprocessingSite } };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        var authorisationTypes = GetAuthorisationTypes();
        var index = authorisationTypes.IndexOf(authorisationTypes.FirstOrDefault(x => x.Id == 1));
        authorisationTypes[index].SelectedAuthorisationText = "testing";

        var model = new SelectAuthorisationTypeViewModel() { SelectedAuthorisation = 1, AuthorisationTypes = authorisationTypes };

        // Act
        var result = _controller.SelectAuthorisationType(model, actionButton);
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;
        // Assert

        backlink.Should().Be(backLinkUrl);
    }

    [TestMethod]
    [DataRow("SaveAndContinue", PagePaths.RegistrationLanding)]
    [DataRow("SaveAndComeBackLater", PagePaths.ApplicationSaved)]
    public async Task SelectAuthorisationType_OnSubmit_ShouldBeSuccessful(string actionButton, string expectedRedirectUrl)
    {
        //Arrange
        _session = new ReprocessorExporterRegistrationSession() { Journey = new List<string> { PagePaths.CountryOfReprocessingSite, PagePaths.GridReferenceOfReprocessingSite } };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        var authorisationTypes = GetAuthorisationTypes();
        var index = authorisationTypes.IndexOf(authorisationTypes.FirstOrDefault(x => x.Id == 1));
        authorisationTypes[index].SelectedAuthorisationText = "testing";

        var model = new SelectAuthorisationTypeViewModel() { SelectedAuthorisation = 1, AuthorisationTypes = authorisationTypes };

        // Act
        var result = _controller.SelectAuthorisationType(model, actionButton);
        var redirectResult = result as RedirectResult;
        // Assert

        // Assert
        using (new AssertionScope())
        {
            redirectResult.Should().NotBeNull();
            redirectResult.Url.Should().Be(expectedRedirectUrl);
        }
    }

    [TestMethod]
    [DataRow(1, "error_message_enter_permit_or_license_number")]
    [DataRow(2, "error_message_enter_permit_number")]
    [DataRow(3, "error_message_enter_permit_number")]
    [DataRow(4, "error_message_enter_permit_number")]
    public async Task SelectAuthorisationType_OnSubmit_ValidateModel_ShouldReturnModelError(int id, string expectedErrorMessage)
    {
        //Arrange
        _session = new ReprocessorExporterRegistrationSession() { Journey = new List<string> { PagePaths.CountryOfReprocessingSite, PagePaths.GridReferenceOfReprocessingSite } };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        var authorisationTypes = GetAuthorisationTypes();
        var model = new SelectAuthorisationTypeViewModel() { SelectedAuthorisation = id, AuthorisationTypes = authorisationTypes };
        var index = authorisationTypes.IndexOf(authorisationTypes.FirstOrDefault(x => x.Id == id));

        // Act
        var result = _controller.SelectAuthorisationType(model, "SaveAndContinue");
        var modelState = _controller.ModelState;

        // Assert
        using (new AssertionScope())
        {
            Assert.IsTrue(modelState[$"AuthorisationTypes.SelectedAuthorisationText[{index}]"].Errors.Count == 1);
            Assert.AreEqual(expectedErrorMessage, modelState[$"AuthorisationTypes.SelectedAuthorisationText[{index}]"].Errors[0].ErrorMessage);
        }
    }

    [TestMethod]
    public async Task ProvideWasteManagementLicense_ReturnsExpectedViewResult()
    {
        // Act
        var result = _controller.ProvideWasteManagementLicense();
        var viewResult = result as ViewResult;

        // Assert
        using (new AssertionScope())
        {
            Assert.AreSame(typeof(ViewResult), result.GetType(), "Result should be of type ViewResult");
            viewResult.Model.Should().BeOfType<ProvideWasteManagementLicenseViewModel>();
        }
    }

    [TestMethod]
    public async Task ProvideWasteManagementLicense_SetsBackLink_ReturnsExpectedViewResult()
    {
        // Act
        var result = _controller.ProvideWasteManagementLicense();
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;

        // Assert
        using (new AssertionScope())
        {
            Assert.AreSame(typeof(ViewResult), result.GetType(), "Result should be of type ViewResult");
            backlink.Should().Be(PagePaths.PermitForRecycleWaste);
        }
    }


    [TestMethod]
    [DataRow("SaveAndContinue", PagePaths.PermitForRecycleWaste)]
    [DataRow("SaveAndComeBackLater", PagePaths.PermitForRecycleWaste)]
    public async Task ProvideWasteManagementLicense_OnSubmit_ShouldSetBackLink(string actionButton, string backLinkUrl)
    {
        //Arrange
        _session = new ReprocessorExporterRegistrationSession() { Journey = new List<string> { PagePaths.PermitForRecycleWaste, PagePaths.WasteManagementLicense } };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);;

        var model = new ProvideWasteManagementLicenseViewModel() {SelectedFrequency= "PerYear", Weight = "10" };

        // Act
        var result = _controller.ProvideWasteManagementLicense(model, actionButton);
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;
        // Assert

        backlink.Should().Be(backLinkUrl);
    }

    [TestMethod]
    [DataRow("SaveAndContinue", PagePaths.RegistrationLanding)]
    [DataRow("SaveAndComeBackLater", PagePaths.ApplicationSaved)]
    public async Task ProvideWasteManagementLicense_OnSubmit_ShouldBeSuccessful(string actionButton, string expectedRedirectUrl)
    {
        //Arrange
        _session = new ReprocessorExporterRegistrationSession() { Journey = new List<string> { PagePaths.PermitForRecycleWaste, PagePaths.WasteManagementLicense } };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);


        var model = new ProvideWasteManagementLicenseViewModel() { SelectedFrequency = MaterialFrequencyOptions.PerYear.ToString(), Weight = "10"};

        // Act
        var result = _controller.ProvideWasteManagementLicense(model, actionButton);
        var redirectResult = result as RedirectResult;
        // Assert

        // Assert
        using (new AssertionScope())
        {
            redirectResult.Should().NotBeNull();
            redirectResult.Url.Should().Be(expectedRedirectUrl);
        }
    }

    [TestMethod]
    [DataRow(null, "10", "Select if the authorised weight is per year, per month or per week", nameof(ProvideWasteManagementLicenseViewModel.SelectedFrequency))]
    [DataRow("PerYear", "0", "Weight must be a number greater than 0", nameof(ProvideWasteManagementLicenseViewModel.Weight))]
    [DataRow("PerYear", "11000000", "Weight must be a number less than 10,000,000", nameof(ProvideWasteManagementLicenseViewModel.Weight))]
    [DataRow("PerYear", "dsdsd", "Weight must be a number, like 100", nameof(ProvideWasteManagementLicenseViewModel.Weight), true)]
    [DataRow("PerYear", null, "Enter the maximum weight the permit authorises the site to accept and recycle", nameof(ProvideWasteManagementLicenseViewModel.Weight))]
    public async Task ProvideWasteManagementLicense_OnSubmit_ValidateModel_ShouldReturnModelError(string selectedFrequency, string weight, string expectedErrorMessage, string modelStateKey, bool isCustomError = false)
    {
        //Arrange
        _session = new ReprocessorExporterRegistrationSession() { Journey = new List<string> { PagePaths.PermitForRecycleWaste, PagePaths.WasteManagementLicense } };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        var model = new ProvideWasteManagementLicenseViewModel() { SelectedFrequency = selectedFrequency, Weight = weight };

        ValidateViewModel(model);
        // Act
        var result = _controller.ProvideWasteManagementLicense(model, "SaveAndComeBackLater");
        var modelState = _controller.ModelState;
        // Assert

        // Assert
        using (new AssertionScope())
        {
            modelStateKey = isCustomError ? "" : modelStateKey;
            Assert.IsTrue(modelState[modelStateKey].Errors.Count == 1);
            Assert.AreEqual(expectedErrorMessage, modelState[modelStateKey].Errors[0].ErrorMessage);
        }
    }


    private void ValidateViewModel(object Model)
    {
        ValidationContext validationContext = new ValidationContext(Model, null, null);
        List<ValidationResult> validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(Model, validationContext, validationResults, true);
        foreach (ValidationResult validationResult in validationResults)
        {
            _controller.ControllerContext.ModelState.AddModelError(String.Join(", ", validationResult.MemberNames), validationResult.ErrorMessage);
        }
    }

    private void SetUpUserAndSessions()
    {
        _sessionManagerMock.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>())).Returns(Task.FromResult(new ReprocessorExporterRegistrationSession()));

        var claims = new List<Claim>();
        var userData = GetUserData();
        if (userData != null)
        {
            claims.Add(new(ClaimTypes.UserData, JsonConvert.SerializeObject(userData)));
        }

        _userMock.Setup(x => x.Claims).Returns(claims);
        _httpContextMock.Setup(x => x.User).Returns(_userMock.Object);
        _controller.ControllerContext.HttpContext = _httpContextMock.Object;
    }

    private static UserData GetUserData()
    {
        return new UserData
        {
            Id = Guid.NewGuid(),
            Organisations = new()
            {
                new()
                {
                    Name = "Tesr",
                    OrganisationNumber = "123456",
                }
            }
        };
    }

    private List<AuthorisationTypes> GetAuthorisationTypes()
    {

        return new List<AuthorisationTypes> { new()
            {
                Id = 1,
                Name = "Environment permit or waste management license",
                Label = "Enter permit or licence number",
                NationCodeCategory = new List<string>(){ "GB-ENG", "GB-WLS" }
            } , new()
             {
                Id = 2,
                Name = "Installation permit",
                Label = "Enter permit number",
                NationCodeCategory = new List<string>(){ "GB-ENG", "GB-WLS" }
            }, new()
              {
                Id = 3,
                Name = "Pollution, Prevention and Control (PPC) permit",
                Label = "Enter permit number",
                NationCodeCategory = new List<string>(){ "GB-NIR", "GB-SCT" }
            }, new()
               {
                Id = 4,
                Name = "Waste management licence",
                Label = "Enter licence number",
                NationCodeCategory = new List<string>(){ "GB-ENG", "GB-WLS", "GB-NIR", "GB-SCT" }
            },
             new()
               {
                Id = 5,
                Name = "Waste exemption",
                Label = "Waste exemption",
                NationCodeCategory = new List<string>(){ "GB-ENG", "GB-NIR", "GB-SCT", "GB-WLS" }
            }
            };
    }
}