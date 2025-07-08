
using AutoMapper;
using ManualAddressForServiceOfNoticesViewModel = Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney.ManualAddressForServiceOfNoticesViewModel;
using ValidationResult = FluentValidation.Results.ValidationResult;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers.ExporterJourney;

[TestClass]
public class ManualAddressForServiceOfNoticesControllerUnitTests
{
    private Mock<ILogger<ManualAddressForServiceOfNoticesController>> _loggerMock;
    private Mock<ISaveAndContinueService> _saveAndContinueServiceMock;
    private Mock<ISessionManager<ExporterRegistrationSession>> _sessionManagerMock;
    private Mock<IValidationService> _validationServiceMock;
    private Mock<IMapper> _mapperMock;
    private readonly Mock<HttpContext> _httpContextMock = new();
    private readonly Mock<ISession> _session = new();
    private ExporterRegistrationSession _exporterSession;
    protected ITempDataDictionary TempDataDictionary = null!;
    private ManualAddressForServiceOfNoticesController _controller;

    private static readonly string _viewPath = "~/Views/ExporterJourney/AddressForServiceOfNotice/ManualAddressForNotices.cshtml";

    [TestInitialize]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<ManualAddressForServiceOfNoticesController>>();
        _saveAndContinueServiceMock = new Mock<ISaveAndContinueService>();
        _sessionManagerMock = new Mock<ISessionManager<ExporterRegistrationSession>>();
        _validationServiceMock = new Mock<IValidationService>();
        _mapperMock = new Mock<IMapper>();

        _httpContextMock.Setup(x => x.Session).Returns(_session.Object);

        _exporterSession = new ExporterRegistrationSession
        {
            RegistrationId = Guid.NewGuid()
        };

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .Returns(Task.FromResult(_exporterSession));

        _sessionManagerMock.Setup(x => x.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<ExporterRegistrationSession>()))
            .Returns(Task.FromResult(true));

        _controller = new ManualAddressForServiceOfNoticesController(
            _loggerMock.Object,
            _saveAndContinueServiceMock.Object,
            _sessionManagerMock.Object,
            _validationServiceMock.Object,
            _mapperMock.Object
        );

        _controller.ControllerContext.HttpContext = _httpContextMock.Object;
        TempDataDictionary = new TempDataDictionary(_httpContextMock.Object, new Mock<ITempDataProvider>().Object);
        _controller.TempData = TempDataDictionary;
    }

    [TestMethod]
    public async Task Get_WhenLegalAddressExists_ReturnsPrepopulatedViewModel()
    {
        _exporterSession.LegalAddress = new()
        {
            AddressLine1 = "1 Test St",
            AddressLine2 = "Apt 2",
            TownCity = "TestTown",
            County = "TestCounty",
            PostCode = "AB12CD"
        };

        var result = await _controller.Get();
        var viewResult = result as ViewResult;

        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as ManualAddressForServiceOfNoticesViewModel;
        Assert.IsNotNull(model);
        Assert.AreEqual("1 Test St", model.AddressLine1);
        Assert.AreEqual("Apt 2", model.AddressLine2);
        Assert.AreEqual("TestTown", model.TownOrCity);
        Assert.AreEqual("TestCounty", model.County);
        Assert.AreEqual("AB12CD", model.Postcode);
    }

    [TestMethod]
    public async Task Get_WhenNoAddressExists_ReturnsEmptyViewModel()
    {
        _exporterSession.LegalAddress = null;

        var result = await _controller.Get();
        var viewResult = result as ViewResult;

        Assert.IsNotNull(viewResult);
        Assert.IsInstanceOfType(viewResult.Model, typeof(ManualAddressForServiceOfNoticesViewModel));
    }

    [TestMethod]
    public async Task Post_WhenModelInvalid_ReturnsSameViewWithErrors()
    {
        var model = new ManualAddressForServiceOfNoticesViewModel();

        _validationServiceMock
            .Setup(v => v.ValidateAsync(It.IsAny<ManualAddressForServiceOfNoticesViewModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure> {
                new ValidationFailure("AddressLine1", "Required")
            }));


        var result = await _controller.Post(model, "SaveAndContinue");

        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        Assert.AreEqual(_viewPath, viewResult.ViewName);
        Assert.AreEqual(model, viewResult.Model);
    }

    [TestMethod]
    public async Task Post_WhenValid_SaveAndContinue_RedirectsToNextPage()
    {
        var model = new ManualAddressForServiceOfNoticesViewModel
        {
            AddressLine1 = "1 Test St",
            TownOrCity = "TestTown",
            County = "TestCounty",
            Postcode = "AB12CD"
        };

        _validationServiceMock
            .Setup(v => v.ValidateAsync(It.IsAny<ManualAddressForServiceOfNoticesViewModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var result = await _controller.Post(model, "SaveAndContinue");

        var redirectResult = result as RedirectResult;
        Assert.IsNotNull(redirectResult);
        Assert.AreEqual(PagePaths.ExporterCheckYourAnswersForNotices, redirectResult.Url);
    }

    [TestMethod]
    public async Task Post_WhenValid_SaveAndComeBackLater_RedirectsToPlaceholder()
    {
        var model = new ManualAddressForServiceOfNoticesViewModel
        {
            AddressLine1 = "1 Test St",
            TownOrCity = "TestTown",
            County = "TestCounty",
            Postcode = "AB12CD"
        };

        _validationServiceMock
            .Setup(v => v.ValidateAsync(It.IsAny<ManualAddressForServiceOfNoticesViewModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var result = await _controller.Post(model, "SaveAndComeBackLater");

        var redirectResult = result as RedirectResult;
        Assert.IsNotNull(redirectResult);
        Assert.AreEqual(PagePaths.ExporterPlaceholder, redirectResult.Url);
    }


    [TestMethod]
    public async Task Post_WhenValid_ConfirmAndContinue_RedirectsToNextPage()
    {
        var model = new ManualAddressForServiceOfNoticesViewModel
        {
            AddressLine1 = "1 Test St",
            TownOrCity = "TestTown",
            County = "TestCounty",
            Postcode = "AB12CD"
        };

        _validationServiceMock
            .Setup(v => v.ValidateAsync(It.IsAny<ManualAddressForServiceOfNoticesViewModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var result = await _controller.Post(model, "ConfirmAndContinue");

        var redirectResult = result as RedirectResult;
        Assert.IsNotNull(redirectResult);
        Assert.AreEqual(PagePaths.ExporterCheckYourAnswersForNotices, redirectResult.Url);
    }

    [TestMethod]
    public async Task Post_WhenValid_CallsPersistJourneyAndSession()
    {
        var model = new ManualAddressForServiceOfNoticesViewModel
        {
            AddressLine1 = "1 Test St",
            TownOrCity = "TestTown",
            County = "TestCounty",
            Postcode = "AB12CD"
        };

        _validationServiceMock
            .Setup(v => v.ValidateAsync(It.IsAny<ManualAddressForServiceOfNoticesViewModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var controller = new ManualAddressForServiceOfNoticesController(
            _loggerMock.Object,
            _saveAndContinueServiceMock.Object,
            _sessionManagerMock.Object,
            _validationServiceMock.Object,
            _mapperMock.Object
        );
        controller.ControllerContext.HttpContext = _httpContextMock.Object;
        controller.TempData = TempDataDictionary;

        await controller.Post(model, "SaveAndContinue");

        _sessionManagerMock.Verify(x => x.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<ExporterRegistrationSession>()), Times.Once);

    }
}
