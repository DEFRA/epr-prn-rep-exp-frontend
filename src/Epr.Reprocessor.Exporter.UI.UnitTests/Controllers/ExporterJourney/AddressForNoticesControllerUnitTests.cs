using AutoMapper;
using Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney;
using ValidationResult = FluentValidation.Results.ValidationResult;
using ValidationFailure = FluentValidation.Results.ValidationFailure;
using Address = Epr.Reprocessor.Exporter.UI.App.Domain.Address;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers.ExporterJourney;

[TestClass]
public class AddressForNoticesControllerUnitTests
{
    private AddressForNoticesController _controller = null!;
    private Mock<ILogger<AddressForNoticesController>> _loggerMock = null!;
    private Mock<ISaveAndContinueService> _saveAndContinueServiceMock = null!;
    private Mock<IReprocessorService> _reprocessorServiceMock = null!;
    private Mock<IValidationService> _validationServiceMock = null!;
    private Mock<ISessionManager<ExporterRegistrationSession>> _sessionManagerMock = null!;
    private Mock<IMapper> _mapperMock;

    private readonly Mock<HttpContext> _httpContextMock = new();
    private readonly Mock<ClaimsPrincipal> _userMock = new();
    private readonly Mock<ISession> _session = new();
    private ExporterRegistrationSession _exporterSession;

    private static readonly string _viewPath = "~/Views/ExporterJourney/AddressForServiceOfNotice/AddressForNotices.cshtml";

    [TestInitialize]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<AddressForNoticesController>>();
        _saveAndContinueServiceMock = new Mock<ISaveAndContinueService>();
        _sessionManagerMock = new Mock<ISessionManager<ExporterRegistrationSession>>();
        _validationServiceMock = new Mock<IValidationService>();
        _reprocessorServiceMock = new Mock<IReprocessorService>();
        _mapperMock = new Mock<IMapper>();

        _httpContextMock.Setup(x => x.Session).Returns(_session.Object);

        _exporterSession = GetExporterRegistration();

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(_exporterSession);

        _sessionManagerMock.Setup(x => x.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<ExporterRegistrationSession>()))
            .Returns(Task.FromResult(true));

        _controller = new AddressForNoticesController(_loggerMock.Object, _saveAndContinueServiceMock.Object, _sessionManagerMock.Object, _validationServiceMock.Object, _mapperMock.Object);

        _controller.ControllerContext.HttpContext = _httpContextMock.Object;

        var userData = GetUserDateWithNationIdAndCompanyNumber();
        var claims = CreateClaims(userData);
        _userMock.Setup(x => x.Claims).Returns(claims);
        _httpContextMock.Setup(x => x.User).Returns(_userMock.Object);
        _controller.ControllerContext.HttpContext = _httpContextMock.Object;
    }

    [TestMethod]
    public async Task Get_Returns_PopulatedView_WhenAddressExists()
    {
        // Act
        var result = await _controller.Get() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        var resultViewModel = result.Model as AddressForNoticesViewModel;
        resultViewModel.SiteAddress.Should().NotBeNull();
        resultViewModel.SiteAddress.AddressLine1.Should().Be("Address line 1");
    }

    [TestMethod]
    public async Task Get_Returns_EmptyAddress_WhenNoAddressExists()
    {
        // Arrange
        _exporterSession.RegistrationApplicationSession.ReprocessingSite.Address = null;

        // Act
        var result = await _controller.Get() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        var resultViewModel = result.Model as AddressForNoticesViewModel;
        resultViewModel.SiteAddress.AddressLine1.Should().BeEmpty();
        resultViewModel.SiteAddress.AddressLine2.Should().BeEmpty();
        resultViewModel.SiteAddress.Postcode.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Post_WhenModelInvalid_ReturnsViewWithErrors()
    {
        // Arrange
        var model = new AddressForNoticesViewModel();

        _validationServiceMock
            .Setup(v => v.ValidateAsync(It.IsAny<AddressForNoticesViewModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure> {
                new ValidationFailure("AddressLine1", "Required")
            }));

        // Act
        var result = await _controller.Post(model) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        var modelState = _controller.ModelState;
        modelState.Should().NotBeNull();
        modelState.Keys.Count().Should().Be(1);
        modelState.Keys.Contains("AddressLine1");
        modelState["AddressLine1"].Errors.Count().Should().Be(1);
        modelState["AddressLine1"].Errors[0].ErrorMessage.Should().Be("Required");
    }

    [TestMethod]
    [DataRow(AddressOptions.BusinessAddress)]
    [DataRow(AddressOptions.RegisteredAddress)]
    public async Task Post_BusinessOrRegisteredAddress_RedirectsTo_TaskList(AddressOptions selectedAddressOption)
    {
        // Arrange
        var model = new AddressForNoticesViewModel();
        model.SelectedAddressOptions = selectedAddressOption;

        _validationServiceMock
            .Setup(v => v.ValidateAsync(It.IsAny<AddressForNoticesViewModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>()));

        // Act
        var result = await _controller.Post(model); 

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Url.Should().NotBeNullOrEmpty();
        redirectResult.Url.Should().Be(PagePaths.ExporterCheckYourAnswersForNotices);
    }

    [TestMethod]
    [DataRow(AddressOptions.DifferentAddress)]
    public async Task Post_DifferentAddress_RedirectsTo_ExporterPostcodeForServiceOfNotices(AddressOptions selectedAddressOption)
    {
        // Arrange
        var model = new AddressForNoticesViewModel();
        model.SelectedAddressOptions = selectedAddressOption;

        _validationServiceMock
            .Setup(v => v.ValidateAsync(It.IsAny<AddressForNoticesViewModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>()));

        // Act
        var result = await _controller.Post(model);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Url.Should().NotBeNullOrEmpty();
        redirectResult.Url.Should().Be(PagePaths.ExporterPostcodeForServiceOfNotices);
    }

    [TestMethod]
    public async Task Post_WhenValid_AndDifferentAddress_PersistsJourneyAndSession()
    {
        // Arrange
        var model = new AddressForNoticesViewModel();
        model.SelectedAddressOptions = AddressOptions.DifferentAddress;

        _validationServiceMock
            .Setup(v => v.ValidateAsync(It.IsAny<AddressForNoticesViewModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>()));

        // Act
        var result = await _controller.Post(model);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<RedirectResult>();
        _sessionManagerMock.Verify(x => x.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<ExporterRegistrationSession>()), Times.Once);
    }

    private static ExporterRegistrationSession GetExporterRegistration()
    {
        return new ExporterRegistrationSession
        {
            RegistrationId = Guid.NewGuid(),
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingSite = new ReprocessingSite
                {
                    Address = new Address("Address line 1", "Address line 2", "Locality", "Town", "County", "Country", "CV12TT")
                },
            }
        };
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

    private static UserData GetUserDateWithNationIdAndCompanyNumber()
    {
        var userData = GetUserData();
        userData.Organisations[0].CompaniesHouseNumber = "123456";
        userData.Organisations[0].NationId = 1;

        return userData;
    }

    private static List<Claim> CreateClaims(UserData? userData)
    {
        var claims = new List<Claim>();
        if (userData != null)
        {
            claims.Add(new(ClaimTypes.UserData, JsonConvert.SerializeObject(userData)));
        }

        return claims;
    }
}
