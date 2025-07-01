using EPR.Common.Authorization.Extensions;
using Organisation = EPR.Common.Authorization.Models.Organisation;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers.Reprocessor;

[TestClass]
public class ReprocessingInputsAndOutputsControllerTests
{
    private ReprocessingInputsAndOutputsController _controller = null!;
    private Mock<IReprocessorService> _reprocessorService = null!;
    private Mock<IPostcodeLookupService> _postcodeLookupService = null!;
    private Mock<IValidationService> _validationService = null!;
    private Mock<ISessionManager<ReprocessorRegistrationSession>> _sessionManagerMock = null!;
    private Mock<IRequestMapper> _requestMapper = null!;
    private readonly Mock<HttpContext> _httpContextMock = new();
    private Mock<IRegistrationMaterialService> _registrationMaterialServiceMock;
    private new Mock<IAccountServiceApiClient> _accountServiceMock;
    
    [TestInitialize]
    public void Setup()
    {
        // ResourcesPath should be 'Resources' but build environment differs from development environment
        // Work around = set ResourcesPath to non-existent location and test for resource keys rather than resource values
        var options = Options.Create(new LocalizationOptions { ResourcesPath = "Resources_not_found" });
        var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
        var localizer = new StringLocalizer<SelectAuthorisationType>(factory);

        _sessionManagerMock = new Mock<ISessionManager<ReprocessorRegistrationSession>>();
        _reprocessorService = new Mock<IReprocessorService>();
        _postcodeLookupService = new Mock<IPostcodeLookupService>();
        _validationService = new Mock<IValidationService>();
        _requestMapper = new Mock<IRequestMapper>();
        _registrationMaterialServiceMock = new Mock<IRegistrationMaterialService>();
        _accountServiceMock = new Mock<IAccountServiceApiClient>();

        CreateSessionMock();
        CreateUserData();

        _controller = new ReprocessingInputsAndOutputsController(_sessionManagerMock.Object, _registrationMaterialServiceMock.Object, _accountServiceMock.Object, _reprocessorService.Object, _postcodeLookupService.Object, _validationService.Object, localizer, _requestMapper.Object);
        _controller.ControllerContext.HttpContext = _httpContextMock.Object;
    }

    [TestMethod]
    public async Task ApplicationContactNameGet_WhenSessionExists_ShouldReturnViewWithModel()
    {
        // Act
        var result = await _controller.ApplicationContactName();
        
        // Assert
        result.Should().BeOfType<ViewResult>();

        var viewResult = (ViewResult)result;
        var model = viewResult.Model as ApplicationContactNameViewModel;
        model.Should().NotBeNull();
    }

    [TestMethod]
    public async Task ApplicationContactNameGet_WhenSessionDoesNotExist_ShouldRedirectToTaskList()
    {
        // Arrange
        _sessionManagerMock
            .Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync((ReprocessorRegistrationSession)null);

        // Act
        var result = await _controller.ApplicationContactName();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = (RedirectResult)result;
        redirectResult.Url.Should().Be(PagePaths.TaskList);
    }

    [TestMethod]
    public async Task ApplicationContactNamePost_WhenSessionDoesNotExist_ShouldRedirectToTaskList()
    {
        // Arrange
        var viewModel = new ApplicationContactNameViewModel();

        _sessionManagerMock
            .Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync((ReprocessorRegistrationSession)null);
        
        // Act
        var result = await _controller.ApplicationContactName(viewModel, "SaveAndContinue");

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = (RedirectResult)result;
        redirectResult.Url.Should().Be(PagePaths.TaskList);
    }

    [TestMethod]
    public async Task ApplicationContactNamePost_WhenModelStateError_ShouldRedisplayView()
    {
        // Arrange
        var viewModel = new ApplicationContactNameViewModel();

        _controller.ModelState.AddModelError("Some error", "some error");

        // Act
        var result = await _controller.ApplicationContactName(viewModel, "SaveAndContinue");

        // Assert
        result.Should().BeOfType<ViewResult>();

        var viewResult = (ViewResult)result;
        var model = viewResult.Model as ApplicationContactNameViewModel;
        model.Should().NotBeNull();
    }

    [TestMethod]
    public async Task ApplicationContactNamePost_WhenButtonActionIsContinue_ShouldRedirectToNextPage()
    {
        // Arrange
        var viewModel = new ApplicationContactNameViewModel { SelectedContact = Guid.NewGuid() };

        // Act
        var result = await _controller.ApplicationContactName(viewModel, "SaveAndContinue");

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();

        var redirectResult = (RedirectToActionResult)result;
        redirectResult.ActionName.Should().Be("TypeOfSuppliers");
    }

    [TestMethod]
    public async Task ApplicationContactNamePost_WhenButtonActionIsComeBackLater_ShouldRedirectToApplicationSaved()
    {
        // Arrange
        var viewModel = new ApplicationContactNameViewModel { SelectedContact = Guid.NewGuid() };

        // Act
        var result = await _controller.ApplicationContactName(viewModel, "SaveAndComeBackLater");

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = (RedirectResult)result;
        redirectResult.Url.Should().Be(PagePaths.ApplicationSaved);
    }

    /// <summary>
    /// //////
    /// 
    /// </summary>
    /// 


    [TestMethod]
    public async Task TypeOfSuppliersGet_WhenSessionExists_ShouldReturnViewWithModel()
    {
        // Act
        var result = await _controller.TypeOfSuppliers();

        // Assert
        result.Should().BeOfType<ViewResult>();

        var viewResult = (ViewResult)result;
        var model = viewResult.Model as TypeOfSuppliersViewModel;
        model.Should().NotBeNull();
    }

    [TestMethod]
    public async Task TypeOfSuppliersGet_WhenSessionDoesNotExist_ShouldRedirectToTaskList()
    {
        // Arrange
        _sessionManagerMock
            .Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync((ReprocessorRegistrationSession)null);

        // Act
        var result = await _controller.TypeOfSuppliers();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = (RedirectResult)result;
        redirectResult.Url.Should().Be(PagePaths.TaskList);
    }

    [TestMethod]
    public async Task TypeOfSuppliersPost_WhenSessionDoesNotExist_ShouldRedirectToTaskList()
    {
        // Arrange
        var viewModel = new TypeOfSuppliersViewModel();

        _sessionManagerMock
            .Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync((ReprocessorRegistrationSession)null);

        // Act
        var result = await _controller.TypeOfSuppliers(viewModel, "SaveAndContinue");

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = (RedirectResult)result;
        redirectResult.Url.Should().Be(PagePaths.TaskList);
    }

    [TestMethod]
    public async Task TypeOfSuppliersPost_WhenModelStateError_ShouldRedisplayView()
    {
        // Arrange
        var viewModel = new TypeOfSuppliersViewModel();

        _controller.ModelState.AddModelError("Some error", "some error");

        // Act
        var result = await _controller.TypeOfSuppliers(viewModel, "SaveAndContinue");

        // Assert
        result.Should().BeOfType<ViewResult>();

        var viewResult = (ViewResult)result;
        var model = viewResult.Model as TypeOfSuppliersViewModel;
        model.Should().NotBeNull();
    }

    [TestMethod]
    public async Task TypeOfSuppliersPost_WhenButtonActionIsContinue_ShouldRedirectToNextPage()
    {
        // Arrange
        var viewModel = new TypeOfSuppliersViewModel { TypeOfSuppliers = "Supplier 123" };

        // Act
        var result = await _controller.TypeOfSuppliers(viewModel, "SaveAndContinue");

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();

        var redirectResult = (RedirectToActionResult)result;
        redirectResult.ActionName.Should().Be("TypeOfSuppliers");
    }

    [TestMethod]
    public async Task TypeOfSuppliersPost_WhenButtonActionIsComeBackLater_ShouldRedirectToApplicationSaved()
    {
        // Arrange
        var viewModel = new TypeOfSuppliersViewModel { TypeOfSuppliers = "Supplier 123" };

        // Act
        var result = await _controller.TypeOfSuppliers(viewModel, "SaveAndComeBackLater");

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = (RedirectResult)result;
        redirectResult.Url.Should().Be(PagePaths.ApplicationSaved);
    }

    private void CreateUserData()
    {
        var claimsIdentity = new ClaimsIdentity();
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        claimsPrincipal.AddOrUpdateUserData(new UserData
        {
            Organisations =
            [
                new Organisation { Id = Guid.NewGuid() }
            ]
        });

        _httpContextMock.Setup(mock => mock.User).Returns(claimsPrincipal);
    }

    private void CreateSessionMock()
    {
        var currentMaterial = new RegistrationMaterialDto();
        var session  = new ReprocessorRegistrationSession { RegistrationApplicationSession = new RegistrationApplicationSession
        {
            ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs { CurrentMaterial = currentMaterial }
        }};

        _sessionManagerMock
        .Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);
    }
}