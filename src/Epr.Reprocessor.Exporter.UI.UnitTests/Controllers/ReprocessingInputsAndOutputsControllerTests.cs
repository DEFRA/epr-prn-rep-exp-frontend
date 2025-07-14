using EPR.Common.Authorization.Extensions;
using Epr.Reprocessor.Exporter.UI.App.Extensions;
using Organisation = EPR.Common.Authorization.Models.Organisation;
using Epr.Reprocessor.Exporter.UI.Validations.ReprocessingInputsAndOutputs;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers;

[TestClass]
public class ReprocessingInputsAndOutputsControllerTests
{
    private Mock<ISessionManager<ReprocessorRegistrationSession>> _sessionManagerMock;
    private Mock<IReprocessorService> _reprocessorServiceMock;
    private Mock<IPostcodeLookupService> _postcodeLookupServiceMock;
    private Mock<IValidationService> _validationServiceMock;
    private Mock<IRequestMapper> _requestMapperMock;
    private Mock<HttpContext> _httpContextMock;
    private Mock<IRegistrationMaterialService> _registrationMaterialServiceMock;
    private Mock<IAccountServiceApiClient> _accountServiceMock;
    private ReprocessingInputsAndOutputs _reprocessingInputsAndOutputsSession;
    private Mock<IOrganisationAccessor> _mockOrganisationAccessor = null!;
    private ReprocessingInputsAndOutputsController _controller;

    [TestInitialize]
    public void SetUp()
    {
        _sessionManagerMock = new Mock<ISessionManager<ReprocessorRegistrationSession>>();
        _reprocessorServiceMock = new Mock<IReprocessorService>();
        _postcodeLookupServiceMock = new Mock<IPostcodeLookupService>();
        _validationServiceMock = new Mock<IValidationService>();
        _requestMapperMock = new Mock<IRequestMapper>();
        _registrationMaterialServiceMock = new Mock<IRegistrationMaterialService>();
        _accountServiceMock = new Mock<IAccountServiceApiClient>();
        _mockOrganisationAccessor = new Mock<IOrganisationAccessor>();
        _httpContextMock = new Mock<HttpContext>();

        _reprocessingInputsAndOutputsSession = CreateSessionMock();
        CreateUserData();

        _controller = new ReprocessingInputsAndOutputsController(
            _sessionManagerMock.Object,
            _registrationMaterialServiceMock.Object,
            _accountServiceMock.Object,
            _reprocessorServiceMock.Object,
            _postcodeLookupServiceMock.Object,
            _validationServiceMock.Object,
            _requestMapperMock.Object,
            _mockOrganisationAccessor.Object
        );

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContextMock.Object
        };
    }

    [TestMethod]
    public async Task PackagingWasteWillReprocess_Get_WhenSessionIsValid_ShouldReturnViewWithModel()
    {
        // Arrange: 
        var session = new ReprocessorRegistrationSession
        {
            RegistrationId = Guid.NewGuid(),
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs()
            }
        };

        _sessionManagerMock.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        var registrationMaterials = new List<RegistrationMaterialDto>
        {
        new RegistrationMaterialDto { MaterialLookup = new MaterialLookupDto { Name = Material.Plastic } },
        new RegistrationMaterialDto { MaterialLookup = new MaterialLookupDto { Name = Material.Wood } }
        };

        _reprocessorServiceMock.Setup(rs => rs.RegistrationMaterials.GetAllRegistrationMaterialsForReprocessingInputsAndOutputsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(registrationMaterials);

        // Act: 
        var result = await _controller.PackagingWasteWillReprocess();

        // Assert:
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        Assert.AreEqual("PackagingWasteWillReprocess", viewResult.ViewName);

        var model = viewResult.Model as PackagingWasteWillReprocessViewModel;
        Assert.IsNotNull(model);
        Assert.AreEqual(2, model.Materials.Count);
    }

    [TestMethod]
    public async Task PackagingWasteWillReprocess_Get_WhenSessionIsNull_ShouldCreateNewSessionAndReturnView()
    {
        // Arrange
        _sessionManagerMock.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync((ReprocessorRegistrationSession)null);

        var registrationMaterials = new List<RegistrationMaterialDto>
        {
        new RegistrationMaterialDto { MaterialLookup = new MaterialLookupDto { Name = Material.Plastic } },
        new RegistrationMaterialDto { MaterialLookup = new MaterialLookupDto { Name = Material.Wood } }
        };

        _reprocessorServiceMock.Setup(rs => rs.RegistrationMaterials.GetAllRegistrationMaterialsForReprocessingInputsAndOutputsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(registrationMaterials);

        // Act
        var result = await _controller.PackagingWasteWillReprocess();

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        Assert.AreEqual("PackagingWasteWillReprocess", viewResult.ViewName);

        var model = viewResult.Model as PackagingWasteWillReprocessViewModel;
        Assert.IsNotNull(model);
        Assert.AreEqual(2, model.Materials.Count);
    }

    [TestMethod]
    public async Task PackagingWasteWillReprocess_Get_WhenSessionHaveOnlyOneMaterial_ShouldRedirectToApplicationContactName()
    {
        // Arrange: 
        var session = new ReprocessorRegistrationSession
        {
            RegistrationId = Guid.NewGuid(),
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs()
            }
        };

        _sessionManagerMock.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        var registrationMaterials = new List<RegistrationMaterialDto>
        {
            new RegistrationMaterialDto { MaterialLookup = new MaterialLookupDto { Name = Material.Plastic } }
        };

        _reprocessorServiceMock.Setup(rs => rs.RegistrationMaterials.GetAllRegistrationMaterialsForReprocessingInputsAndOutputsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(registrationMaterials);

        // Act: 
        var result = await _controller.PackagingWasteWillReprocess();

        // Assert:
        var redirectResult = result as RedirectResult;
        Assert.AreEqual(PagePaths.ApplicationContactName, redirectResult.Url);
    }

    [TestMethod]
    public async Task PackagingWasteWillReprocess_Post_WhenModelStateInvalid_ShouldReturnViewWithModel()
    {
        // Arrange: 
        var session = new ReprocessorRegistrationSession
        {
            RegistrationId = Guid.NewGuid(),
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs()
            }
        };

        session.RegistrationApplicationSession.ReprocessingInputsAndOutputs.Materials = new List<RegistrationMaterialDto>
        {
            new RegistrationMaterialDto { MaterialLookup = new MaterialLookupDto { Name = Material.Plastic } }
        };

        var model = new PackagingWasteWillReprocessViewModel();
        var buttonAction = "SaveAndContinue";

        var validationResult = new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure>
            {
                new()
                {
                     PropertyName = "SelectedRegistrationMaterials",
                     ErrorMessage = "Select the packaging waste you’ll reprocess",
                }
            });

        _validationServiceMock.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        _sessionManagerMock.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);
        _controller.ModelState.AddModelError("Key", "Error");

        // Act: 
        var result = await _controller.PackagingWasteWillReprocess(model, buttonAction);

        // Assert:
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult, "Expected a ViewResult, but got null.");
        Assert.AreEqual("PackagingWasteWillReprocess", viewResult.ViewName);

        var returnedModel = viewResult.Model as PackagingWasteWillReprocessViewModel;
        Assert.IsNotNull(returnedModel, "Expected a model, but got null.");
    }

    [TestMethod]
    public async Task PackagingWasteWillReprocess_Post_WhenButtonActionSaveAndContinue_ShouldRedirectToReasonNotReprocessing()
    {
        // Arrange:
        var session = new ReprocessorRegistrationSession
        {
            RegistrationId = Guid.NewGuid(),
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs()
            }
        };

        var model = new PackagingWasteWillReprocessViewModel
        {
            SelectedRegistrationMaterials = new List<string> { "Plastic" }
        };

        _validationServiceMock.Setup(v => v.ValidateAsync(model, CancellationToken.None))
.ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _sessionManagerMock.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);
        _reprocessorServiceMock.Setup(rs => rs.RegistrationMaterials.GetAllRegistrationMaterialsForReprocessingInputsAndOutputsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<RegistrationMaterialDto>
            {
            new RegistrationMaterialDto { MaterialLookup = new MaterialLookupDto { Name = Material.Plastic } }
            });

        _controller.ModelState.Clear();

        var buttonAction = "SaveAndContinue";

        // Act: 
        var result = await _controller.PackagingWasteWillReprocess(model, buttonAction);

        // Assert: 
        var redirectResult = result as RedirectResult;
        Assert.AreEqual(PagePaths.ReasonNotReprocessing, redirectResult.Url);
    }

    [TestMethod]
    public async Task PackagingWasteWillReprocess_Post_WhenButtonActionSaveAndContinue_ShouldRedirectToApplicationContactName()
    {
        // Arrange: 
        var session = new ReprocessorRegistrationSession
        {
            RegistrationId = Guid.NewGuid(),
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs
                {
                    Materials = new List<RegistrationMaterialDto>
                {
                    new RegistrationMaterialDto { MaterialLookup = new MaterialLookupDto { Name = Material.Plastic }, IsMaterialBeingAppliedFor = true }
                }
                }
            }
        };

        var model = new PackagingWasteWillReprocessViewModel
        {
            SelectedRegistrationMaterials = new List<string> { "Plastic" }
        };

        _validationServiceMock.Setup(v => v.ValidateAsync(model, CancellationToken.None))
.ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _sessionManagerMock.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);
        _reprocessorServiceMock.Setup(rs => rs.RegistrationMaterials.GetAllRegistrationMaterialsForReprocessingInputsAndOutputsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(session.RegistrationApplicationSession.ReprocessingInputsAndOutputs.Materials);

        _controller.ModelState.Clear();

        var buttonAction = "SaveAndContinue";

        // Act: 
        var result = await _controller.PackagingWasteWillReprocess(model, buttonAction);

        // Assert: 
        var redirectResult = result as RedirectResult;
        Assert.AreEqual(PagePaths.ApplicationContactName, redirectResult.Url);
    }

    [TestMethod]
    public async Task PackagingWasteWillReprocess_Post_WhenButtonActionSaveAndComeBackLater_ShouldRedirectToApplicationSaved()
    {
        // Arrange: 
        var session = new ReprocessorRegistrationSession { RegistrationId = Guid.NewGuid() };
        var model = new PackagingWasteWillReprocessViewModel();
        var buttonAction = "SaveAndComeBackLater";

        _validationServiceMock.Setup(v => v.ValidateAsync(model, CancellationToken.None))
.ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _sessionManagerMock.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);
        _reprocessorServiceMock.Setup(rs => rs.RegistrationMaterials.GetAllRegistrationMaterialsForReprocessingInputsAndOutputsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(session.RegistrationApplicationSession.ReprocessingInputsAndOutputs.Materials);

        // Act: 
        var result = await _controller.PackagingWasteWillReprocess(model, buttonAction);

        // Assert: 
        var redirectResult = result as RedirectResult;
        Assert.AreEqual(PagePaths.ApplicationSaved, redirectResult.Url);
    }

    [TestMethod]
    public async Task PackagingWasteWillReprocess_Post_WhenSessionIsNull_ShouldUseNewReprocessorRegistrationSession()
    {
        // Arrange
        _sessionManagerMock.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync((ReprocessorRegistrationSession)null);

        var model = new PackagingWasteWillReprocessViewModel();
        var buttonAction = "SaveAndContinue";

        var validationResult = new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure>
            {
                new()
                {
                     PropertyName = "SelectedRegistrationMaterials",
                     ErrorMessage = "Select the packaging waste you’ll reprocess",
                }
            });

        _validationServiceMock.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(validationResult);

        _controller.ModelState.AddModelError("key", "error");

        // Act
        var result = await _controller.PackagingWasteWillReprocess(model, buttonAction);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        Assert.AreEqual("PackagingWasteWillReprocess", viewResult.ViewName);

        var returnedModel = viewResult.Model as PackagingWasteWillReprocessViewModel;
        Assert.IsNotNull(returnedModel);
    }

    [TestMethod]
    public async Task PackagingWasteWillReprocess_Post_WhenButtonActionIsUnknown_ShouldReturnViewWithModel()
    {
        // Arrange
        var session = new ReprocessorRegistrationSession
        {
            RegistrationId = Guid.NewGuid(),
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs
                {
                    Materials = new List<RegistrationMaterialDto>
                {
                    new RegistrationMaterialDto
                    {
                        MaterialLookup = new MaterialLookupDto { Name = Material.Plastic }
                    }
                }
                }
            }
        };

        var model = new PackagingWasteWillReprocessViewModel
        {
            SelectedRegistrationMaterials = new List<string> { "Plastic" }
        };

        _validationServiceMock.Setup(v => v.ValidateAsync(model, CancellationToken.None))
    .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _sessionManagerMock.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        _reprocessorServiceMock.Setup(rs => rs.RegistrationMaterials.GetAllRegistrationMaterialsForReprocessingInputsAndOutputsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(session.RegistrationApplicationSession.ReprocessingInputsAndOutputs.Materials);

        _controller.ModelState.Clear();

        var buttonAction = "SomeUnknownAction";

        // Act
        var result = await _controller.PackagingWasteWillReprocess(model, buttonAction);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);

        var returnedModel = viewResult.Model as PackagingWasteWillReprocessViewModel;
        Assert.IsNotNull(returnedModel);
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
    public async Task ApplicationContactNameGet_WhenSingleMaterial_ShouldSetBackToTaskList()
    {
        // Arrange
        _reprocessingInputsAndOutputsSession.Materials = [new RegistrationMaterialDto()];

        // Act
        var result = await _controller.ApplicationContactName();

        // Assert
        result.Should().BeOfType<ViewResult>();

        var viewResult = (ViewResult)result;

        viewResult.ViewData["BackLinkToDisplay"].Should().Be(PagePaths.TaskList);
    }

    [TestMethod]
    public async Task ApplicationContactNameGet_WhenMultipleMaterials_ShouldSetBackToPackagingWasteWillReprocess()
    {
        // Arrange
        _reprocessingInputsAndOutputsSession.Materials =
        [
            new RegistrationMaterialDto(),
            new RegistrationMaterialDto()
        ];

        // Act
        var result = await _controller.ApplicationContactName();

        // Assert
        result.Should().BeOfType<ViewResult>();

        var viewResult = (ViewResult)result;

        viewResult.ViewData["BackLinkToDisplay"].Should().Be(PagePaths.PackagingWasteWillReprocess);
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
    public async Task TypeOfSuppliersGet_WhenTypeOfSuppliersExists_ShouldReturnViewWithModelMapped()
    {
        // Arrange
        var currentMaterial = new RegistrationMaterialDto
        {
            MaterialLookup = new MaterialLookupDto { Name = Material.Plastic },
            RegistrationReprocessingIO = new RegistrationReprocessingIODto
            {
                TypeOfSuppliers = "Supplier 123"
            }
        };

        var session = new ReprocessorRegistrationSession
        {
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs
                {
                    CurrentMaterial = currentMaterial
                }
            }
        };

        _sessionManagerMock
            .Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Act
        var result = await _controller.TypeOfSuppliers();

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = (ViewResult)result;
        var model = viewResult.Model as TypeOfSuppliersViewModel;
        model.Should().NotBeNull();
        model.TypeOfSuppliers.Equals("Supplier 123");
        model.MaterialName.Equals(Material.Plastic.GetDisplayName());
    }

    [TestMethod]
    public async Task TypeOfSuppliersGet_WhenRegistrationReprocessingIONotExists_ShouldReturnViewWithModelMapped()
    {
        // Arrange

        var currentMaterial = new RegistrationMaterialDto
        {
            RegistrationReprocessingIO = null
        };

        var session = new ReprocessorRegistrationSession
        {
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs
                {
                    CurrentMaterial = currentMaterial
                }
            }
        };

        _sessionManagerMock
            .Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Act
        var result = await _controller.TypeOfSuppliers();

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = (ViewResult)result;
        var model = viewResult.Model as TypeOfSuppliersViewModel;
        model.Should().NotBeNull();
        model.TypeOfSuppliers.Should().BeNull();
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
    public async Task TypeOfSuppliersPost_WhenModelIsInvalid_ShouldReturnViewWithMappedModel()
    {
        // Arrange
        var currentMaterial = new RegistrationMaterialDto
        {
            MaterialLookup = new MaterialLookupDto { Name = Material.Plastic },
            RegistrationReprocessingIO = new RegistrationReprocessingIODto
            {
                TypeOfSuppliers = "Supplier 123"
            }
        };

        var session = new ReprocessorRegistrationSession
        {
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs
                {
                    CurrentMaterial = currentMaterial
                }
            }
        };

        _sessionManagerMock
            .Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Make ModelState invalid
        _controller.ModelState.AddModelError("SomeProperty", "Error");

        var viewModel = new TypeOfSuppliersViewModel();

        // Act
        var result = await _controller.TypeOfSuppliers(viewModel, "SaveAndContinue");

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = (ViewResult)result;
        viewResult.Model.Should().BeOfType<TypeOfSuppliersViewModel>();
        var model = viewResult.Model as TypeOfSuppliersViewModel;
        model.TypeOfSuppliers.Equals("Supplier 123");
        model.MaterialName.Equals(Material.Plastic.GetDisplayName());
    }

    [TestMethod]
    public async Task TypeOfSuppliersPost_WhenModelIsInvalid_AndWhenRegistrationReprocessingIONotExists_ShouldReturnViewWithMappedModel()
    {
        // Arrange
        var currentMaterial = new RegistrationMaterialDto
        {
            RegistrationReprocessingIO = null
        };

        var session = new ReprocessorRegistrationSession
        {
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs
                {
                    CurrentMaterial = currentMaterial
                }
            }
        };

        _sessionManagerMock
            .Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Make ModelState invalid
        _controller.ModelState.AddModelError("SomeProperty", "Error");

        var viewModel = new TypeOfSuppliersViewModel();

        // Act
        var result = await _controller.TypeOfSuppliers(viewModel, "SaveAndContinue");

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = (ViewResult)result;
        viewResult.Model.Should().BeOfType<TypeOfSuppliersViewModel>();
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
        redirectResult.ActionName.Should().Be("LastCalendarYearFlag");
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

    // TODO
    [TestMethod]
    public async Task InputLastCalenderYearGet_WhenSessionExists_ShouldReturnViewWithModel()
    {
        // Act
        var result = await _controller.InputsForLastCalendarYear();

        // Assert
        result.Should().BeOfType<ViewResult>();

        var viewResult = (ViewResult)result;
        var model = viewResult.Model as InputsForLastCalendarYearViewModel;
        model.Should().NotBeNull();
    }

    [TestMethod]
    public async Task InputLastCalenderYearGet_WhenSessionDoesNotExist_ShouldRedirectToTaskList()
    {
        // Arrange
        _sessionManagerMock
            .Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync((ReprocessorRegistrationSession)null);

        // Act
        var result = await _controller.InputsForLastCalendarYear();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = (RedirectResult)result;
        redirectResult.Url.Should().Be(PagePaths.TaskList);
    }

    [TestMethod]
    public async Task InputLastCalenderYearPost_WhenSessionDoesNotExist_ShouldRedirectToTaskList()
    {
        // Arrange
        var viewModel = new InputsForLastCalendarYearViewModel();

        _sessionManagerMock
            .Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync((ReprocessorRegistrationSession)null);

        // Act
        var result = await _controller.InputsForLastCalendarYear(viewModel, "SaveAndContinue");

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = (RedirectResult)result;
        redirectResult.Url.Should().Be(PagePaths.TaskList);
    }

    [TestMethod]
    public async Task InputLastCalenderYearPost_WhenModelStateError_ShouldRedisplayView()
    {
        // Arrange
        var viewModel = new InputsForLastCalendarYearViewModel();
        var validationResult = new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure>
        {
        new()
                {
                     PropertyName = "SelectedRegistrationMaterials",
                     ErrorMessage = "Error in InputLastCalenderYear",
                }
         });
        _validationServiceMock.Setup(v => v.ValidateAsync(viewModel, default))
    .ReturnsAsync(validationResult);

        _controller.ModelState.AddModelError("Some error", "some error");

        // Act
        var result = await _controller.InputsForLastCalendarYear(viewModel, "SaveAndContinue");

        // Assert
        result.Should().BeOfType<ViewResult>();

        var viewResult = (ViewResult)result;
        var model = viewResult.Model as InputsForLastCalendarYearViewModel;
        model.Should().NotBeNull();
    }

    [TestMethod]
    public async Task InputLastCalenderYearPost_WhenButtonActionIsContinue_ShouldRedirectToNextPage()
    {
        // Arrange
        var viewModel = new InputsForLastCalendarYearViewModel();
        var buttonAction = "SaveAndContinue";
        _validationServiceMock.Setup(v => v.ValidateAsync(viewModel, CancellationToken.None))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        // Act
        var result = await _controller.InputsForLastCalendarYear(viewModel, buttonAction);

        using(new AssertionScope())
        {
            // Assert
            result.Should().BeOfType<RedirectToActionResult>();

            var redirectResult = (RedirectToActionResult)result;
            redirectResult.ActionName.Should().Be("ReprocessingOutputsForLastYear");
        }
    }

    [TestMethod]
    public async Task InputLastCalenderYear_Post_WhenButtonActionIsComeBackLater_ShouldRedirectToApplicationSaved()
    {
        // Arrange: 
        var viewModel = new InputsForLastCalendarYearViewModel();
        var buttonAction = "SaveAndComeBackLater";

        _validationServiceMock.Setup(v => v.ValidateAsync(viewModel, CancellationToken.None))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // Act: 
        var result = await _controller.InputsForLastCalendarYear(viewModel, buttonAction);

        // Assert: 
        var redirectResult = result as RedirectResult;
        Assert.AreEqual(PagePaths.ApplicationSaved, redirectResult.Url);
    }

    [TestMethod]
    public async Task InputLastCalenderYearPost_ShouldMap_WasteAndRawMaterials_Valid()
    {
        // Arrange
        var viewModel = new InputsForLastCalendarYearViewModel
        {
            RawMaterials = new List<RawMaterialRowViewModel>
            {
                new RawMaterialRowViewModel
                {
                    RawMaterialName = "Plastic",
                    Tonnes = "45"
            }
        },
            UkPackagingWaste = "11",
            NonUkPackagingWaste = "22",
            NonPackagingWaste = "33",
        };

        var session = new ReprocessorRegistrationSession
        {
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs
                {
                    CurrentMaterial = new RegistrationMaterialDto()
                    {
                        RegistrationReprocessingIO = new RegistrationReprocessingIODto()
                    }
                }
            }
        };

        _sessionManagerMock.Setup(m => m.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);
        _validationServiceMock.Setup(v => v.ValidateAsync(viewModel, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // Act
        var result = await _controller.InputsForLastCalendarYear(viewModel, "SaveAndContinue");

        // Assert (optional, to verify mapping)
        var io = session.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial.RegistrationReprocessingIO;
        io.UKPackagingWasteTonne.Should().Be(11m);     // parsed
        io.NonUKPackagingWasteTonne.Should().Be(22m);      // fallback
        io.NotPackingWasteTonne.Should().Be(33m);          // fallback
        var mapped = io.RegistrationReprocessingIORawMaterialOrProducts;
        mapped.Should().ContainSingle();
        mapped[0].RawMaterialOrProductName.Should().Be("Plastic");
        mapped[0].TonneValue.Should().Be(45m);
        mapped[0].IsInput.Should().BeTrue();
    }

    [TestMethod]
    public async Task InputLastCalenderYearPost_ShouldMap_RawMaterials_InValid()
    {
        var viewModel = new InputsForLastCalendarYearViewModel
        {
            RawMaterials = new List<RawMaterialRowViewModel>
        {
            new RawMaterialRowViewModel
            {
                RawMaterialName = "Glass",
                Tonnes = "invalid"
            }
        }
        };

        var session = new ReprocessorRegistrationSession
        {
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs
                {
                    CurrentMaterial = new RegistrationMaterialDto
                    {
                        RegistrationReprocessingIO = new RegistrationReprocessingIODto()
                    }
                }
            }
        };

        _sessionManagerMock.Setup(m => m.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);
        _validationServiceMock.Setup(v => v.ValidateAsync(viewModel, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var result = await _controller.InputsForLastCalendarYear(viewModel, "SaveAndContinue");

        var mapped = session.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial.RegistrationReprocessingIO.RegistrationReprocessingIORawMaterialOrProducts;
        mapped.Should().ContainSingle();
        mapped[0].RawMaterialOrProductName.Should().Be("Glass");
        mapped[0].TonneValue.Should().Be(0); // fallback confirmed
        mapped[0].IsInput.Should().BeTrue();
    }

    [TestMethod]
    public async Task InputLastCalenderYearPost_ShouldMap_WasteMaterial_Valid()
    {
        // Arrange
        var viewModel = new InputsForLastCalendarYearViewModel
        {
            UkPackagingWaste = "10",
            NonUkPackagingWaste = "20",
            NonPackagingWaste = "5"

        };

        var session = new ReprocessorRegistrationSession
        {
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs
                {
                    CurrentMaterial = new RegistrationMaterialDto()
                    {
                        RegistrationReprocessingIO = new RegistrationReprocessingIODto()
                    }
                }
            }
        };

        _sessionManagerMock.Setup(m => m.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);
        _validationServiceMock.Setup(v => v.ValidateAsync(viewModel, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // Act
        var result = await _controller.InputsForLastCalendarYear(viewModel, "SaveAndContinue");

        // Assert
        var io = session.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial.RegistrationReprocessingIO;

        io.UKPackagingWasteTonne.Should().Be(10m);
        io.NonUKPackagingWasteTonne.Should().Be(20m);
        io.NotPackingWasteTonne.Should().Be(5m);
        io.TotalInputs.Should().Be(35m);
    }

    [TestMethod]
    public async Task InputLastCalenderYearPost_ShouldMap_WasteMaterial_InValid()
    {
        // Arrange
        var viewModel = new InputsForLastCalendarYearViewModel
        {
            UkPackagingWaste = "abc",
            NonUkPackagingWaste = "xyz",
            NonPackagingWaste = "NaN"
        };
        var session = new ReprocessorRegistrationSession
        {
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs
                {
                    CurrentMaterial = new RegistrationMaterialDto()
                    {
                        RegistrationReprocessingIO = new RegistrationReprocessingIODto()
                    }
                }
            }
        };

        _sessionManagerMock.Setup(m => m.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);
        _validationServiceMock.Setup(v => v.ValidateAsync(viewModel, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // Act
        var result = await _controller.InputsForLastCalendarYear(viewModel, "SaveAndContinue");

        // Assert
        var io = session.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial.RegistrationReprocessingIO;

        io.UKPackagingWasteTonne.Should().Be(0);
        io.NonUKPackagingWasteTonne.Should().Be(0);
        io.NotPackingWasteTonne.Should().Be(0);
        io.TotalInputs.Should().Be(0);
    }

    [TestMethod]
    public async Task InputLastCalenderYearPost_ShouldMap_WasteMaterial_ValidInvalid()
    {
        // Arrange
        var viewModel = new InputsForLastCalendarYearViewModel
        {
            UkPackagingWaste = null, // null
            NonUkPackagingWaste = "invalid", // invalid
            NonPackagingWaste = "15" // valid
        };

        var session = new ReprocessorRegistrationSession
        {
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs
                {
                    CurrentMaterial = new RegistrationMaterialDto()
                    {
                        RegistrationReprocessingIO = new RegistrationReprocessingIODto()
                    }
                }
            }
        };

        _sessionManagerMock.Setup(m => m.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);
        _validationServiceMock.Setup(v => v.ValidateAsync(viewModel, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // Act
        var result = await _controller.InputsForLastCalendarYear(viewModel, "SaveAndContinue");

        // Assert
        var io = session.RegistrationApplicationSession.ReprocessingInputsAndOutputs.CurrentMaterial.RegistrationReprocessingIO;

        io.UKPackagingWasteTonne.Should().Be(0);
        io.NonUKPackagingWasteTonne.Should().Be(0);
        io.NotPackingWasteTonne.Should().Be(15m);
        io.TotalInputs.Should().Be(15m);
    }


    [TestMethod]
    public async Task LastCalendarYearFlagGet_WhenSessionExists_ShouldReturnViewWithModel()
    {
        // Act
        var result = await _controller.LastCalendarYearFlag();

        // Assert
        result.Should().BeOfType<ViewResult>();

        var viewResult = (ViewResult)result;
        var model = viewResult.Model as LastCalendarYearFlagViewModel;
        model.Should().NotBeNull();
    }

    [TestMethod]
    public async Task LastCalendarYearFlagGet_WhenSessionDoesNotExist_ShouldRedirectToTaskList()
    {
        // Arrange
        _sessionManagerMock
            .Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync((ReprocessorRegistrationSession)null);

        // Act
        var result = await _controller.LastCalendarYearFlag();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = (RedirectResult)result;
        redirectResult.Url.Should().Be(PagePaths.TaskList);
    }

    [TestMethod]
    public async Task LastCalendarYearFlagPost_WhenButtonActionIsCoontinue_AndNoSelected_ShouldRedirectToEstimateAnnualInputs()
    {
        // Arrange
        var viewModel = new LastCalendarYearFlagViewModel();
        viewModel.ReprocessingPackagingWasteLastYearFlag = false;

        // Act
        var result = await _controller.LastCalendarYearFlag(viewModel, "SaveAndContinue");

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = (RedirectResult)result;
        redirectResult.Url.Should().Be(PagePaths.EstimateAnnualInputs);
    }

    [TestMethod]
    public async Task LastCalendarYearFlagPost_WhenButtonActionIsCoontinue_AndYesSelected_ShouldRedirectToInputLastCalendarYear()
    {
        // Arrange
        var viewModel = new LastCalendarYearFlagViewModel();
        viewModel.ReprocessingPackagingWasteLastYearFlag = true;

        // Act
        var result = await _controller.LastCalendarYearFlag(viewModel, "SaveAndContinue");

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = (RedirectResult)result;
        redirectResult.Url.Should().Be(PagePaths.InputsForLastCalendarYear);
    }

    [TestMethod]
    public async Task LastCalendarYearFlagPost_WhenButtonActionIsComeBackLater_ShouldRedirectToApplicationSaved()
    {
        // Arrange
        var viewModel = new LastCalendarYearFlagViewModel();
        viewModel.ReprocessingPackagingWasteLastYearFlag = true;

        // Act
        var result = await _controller.LastCalendarYearFlag(viewModel, "SaveAndComeBackLater");

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = (RedirectResult)result;
        redirectResult.Url.Should().Be(PagePaths.ApplicationSaved);
    }

    [TestMethod]
    public async Task LastCalendarYearFlagPost_WhenModelStateError_ShouldRedisplayView()
    {
        // Arrange
        var viewModel = new LastCalendarYearFlagViewModel();

        _controller.ModelState.AddModelError("Some error", "some error");

        // Act
        var result = await _controller.LastCalendarYearFlag(viewModel, "SaveAndContinue");

        // Assert
        result.Should().BeOfType<ViewResult>();

        var viewResult = (ViewResult)result;
        var model = viewResult.Model as LastCalendarYearFlagViewModel;
        model.Should().NotBeNull();
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

    private ReprocessingInputsAndOutputs CreateSessionMock()
    {
        var currentMaterial = new RegistrationMaterialDto();
        var reprocessingInputsAndOutputsSession = new ReprocessingInputsAndOutputs { CurrentMaterial = currentMaterial };


        var session = new ReprocessorRegistrationSession
        {
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = reprocessingInputsAndOutputsSession
            }
        };

        _sessionManagerMock
            .Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        return reprocessingInputsAndOutputsSession;
    }
    [TestMethod]
    public async Task ReprocessingOutputsForLastYear_SessionIsNull_RedirectsToTaskList()
    {
        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync((ReprocessorRegistrationSession)null);

        var result = await _controller.ReprocessingOutputsForLastYear();
        var redirectResult = result as RedirectResult;
        Assert.IsNotNull(redirectResult);
        Assert.AreEqual(PagePaths.TaskList, redirectResult.Url);

    }

    [TestMethod]
    public async Task ReprocessingOutputsForLastYear_CurrentMaterialIsNull_RedirectsToTaskList()
    {
        var session = new ReprocessorRegistrationSession
        {
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs
                {
                    CurrentMaterial = null
                }
            }
        };

        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()));
       

        // With this line:
       

        var result = await _controller.ReprocessingOutputsForLastYear();

        var redirectResult = result as RedirectResult;
        Assert.IsNotNull(redirectResult);
        Assert.AreEqual(PagePaths.TaskList, redirectResult.Url);

    }

    [TestMethod]
    public async Task ReprocessingOutputsForLastYear_ValidSessionAndMaterial_ReturnsViewWithModel()
    {
        // Arrange
        var session = new ReprocessorRegistrationSession
        {
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs
                {
                    CurrentMaterial = new RegistrationMaterialDto
                    {
                        MaterialLookup = new MaterialLookupDto { Name = Material.Plastic },
                        RegistrationReprocessingIO = new RegistrationReprocessingIODto { TotalInputs = 200 }
                    }
                }
            }
        };

        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Act
        var result = await _controller.ReprocessingOutputsForLastYear();

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        Assert.AreEqual(nameof(_controller.ReprocessingOutputsForLastYear), viewResult.ViewName);

        var model = viewResult.Model as ReprocessedMaterialOutputSummaryModel;
        Assert.IsNotNull(model);
        Assert.AreEqual("Plastic", model.MaterialName);
        Assert.AreEqual(200, model.TotalInputTonnes);
        Assert.AreEqual(10, model.ReprocessedMaterialsRawData.Count);
    }

    [TestMethod]
    public async Task ReprocessingOutputsForLastYear_TotalInputsIsNull_DefaultsTo100()
    {
        // Arrange
        var session = new ReprocessorRegistrationSession
        {
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs
                {
                    CurrentMaterial = new RegistrationMaterialDto
                    {
                        MaterialLookup = new MaterialLookupDto { Name = Material.Glass },
                        RegistrationReprocessingIO = null
                    }
                }
            }
        };

        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Act
        var result = await _controller.ReprocessingOutputsForLastYear();

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);

        var model = viewResult.Model as ReprocessedMaterialOutputSummaryModel;
        Assert.IsNotNull(model);
        Assert.AreEqual(100, model.TotalInputTonnes);
    }


    [TestMethod]
    public async Task ReprocessingOutputsForLastYear_MaterialNameIsNull_ReturnsViewWithNullName()
    {
        var session = new ReprocessorRegistrationSession
        {
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs
                {
                    CurrentMaterial = new RegistrationMaterialDto
                    {
                        MaterialLookup = new MaterialLookupDto { Name = Material.Glass },
                        RegistrationReprocessingIO = new RegistrationReprocessingIODto { TotalInputs = 200 }
                    }
                }
            }
        };

        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        var result = await _controller.ReprocessingOutputsForLastYear();

        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);

        var model = viewResult.Model as ReprocessedMaterialOutputSummaryModel;
        Assert.IsNotNull(model);
        Assert.AreEqual(200, model.TotalInputTonnes);
    }


    [TestMethod]
    public async Task ReprocessingOutputsForLastYear_Post_Should_Fail_When_Mandatory_Fields_Are_Null()
    {
        // Arrange
        var model = new ReprocessedMaterialOutputSummaryModel
        {
            SentToOtherSiteTonnes = null,
            ContaminantTonnes = null,
            ProcessLossTonnes = null
        };
        var validator = new ReprocessingOutputModelValidator();

        // Act
        var result = validator.Validate(model);

        using (new AssertionScope())
        {
            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "SentToOtherSiteTonnes"));
            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "ContaminantTonnes"));
            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "ProcessLossTonnes"));
        }
        
    }

    [TestMethod]
    public async Task ReprocessingOutputsForLastYear_Post_Should_Fail_When_MaterialOrProductName__Pass_ReprocessTonnes_Null()
    {
        // Arrange
        var model = new ReprocessedMaterialOutputSummaryModel
        {
            SentToOtherSiteTonnes = 10,
            ContaminantTonnes = 5,
            ProcessLossTonnes = 2,

            ReprocessedMaterialsRawData = new List<ReprocessedMaterialRawDataModel>
                {
                    new ReprocessedMaterialRawDataModel
                    {
                        MaterialOrProductName = "Product A",
                        ReprocessedTonnes = 0 // This should trigger validation failure
                    }
                }
        };
        var validator = new ReprocessingOutputModelValidator();

        // Act
        var result = validator.Validate(model);

        using (new AssertionScope())
        {
            // Assert
            Assert.IsFalse(result.IsValid);
            // Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "ReprocessedTonnes"));
            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "ReprocessedMaterialsRawData[0].ReprocessedTonnes.Value"));
        }
        
    }

    [TestMethod]
    public async Task ReprocessingOutputsForLastYear_Post_WhenModelIsValidAndSaveAndContinue_ShouldRedirectToPlantAndEquipment()
    {
        // Arrange
        var session = new ReprocessorRegistrationSession
        {
            RegistrationId = Guid.NewGuid(),
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs
                {
                    CurrentMaterial = new RegistrationMaterialDto
                    {
                        Id = Guid.NewGuid(),
                        MaterialLookup = new MaterialLookupDto { Name = Material.Plastic },
                        RegistrationReprocessingIO = new RegistrationReprocessingIODto()
                    }
                }
            }
        };

        var model = new ReprocessedMaterialOutputSummaryModel
        {
            SentToOtherSiteTonnes = 10,
            ContaminantTonnes = 5,
            ProcessLossTonnes = 2,

            ReprocessedMaterialsRawData = new List<ReprocessedMaterialRawDataModel>
                {
                    new ReprocessedMaterialRawDataModel
                    {
                        MaterialOrProductName = "Product A",
                        ReprocessedTonnes = 3
                    }
                }
        };

        _sessionManagerMock.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        _validationServiceMock.Setup(v => v.ValidateAsync(model, CancellationToken.None))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _registrationMaterialServiceMock.Setup(r => r.UpsertRegistrationReprocessingDetailsAsync(
            It.IsAny<Guid>(),
            It.IsAny<RegistrationReprocessingIODto>()
        )).Returns(Task.CompletedTask);

        var buttonAction = "SaveAndContinue";

        // Act
        var result = await _controller.ReprocessingOutputsForLastYear(model, buttonAction);

        using (new AssertionScope())
        {
            // Assert
            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(PagePaths.PlantAndEquipment, redirectResult.Url);
        }
    }
    [TestMethod]
    public async Task ReprocessingOutputsForLastYear_Post_WhenModelIsValidAndSaveAndComeBackLater_ShouldRedirectToApplicationSaved()
    {
        // Arrange
        var session = new ReprocessorRegistrationSession
        {
            RegistrationId = Guid.NewGuid(),
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs
                {
                    CurrentMaterial = new RegistrationMaterialDto
                    {
                      
                        MaterialLookup = new MaterialLookupDto { Name = Material.Plastic },
                        RegistrationReprocessingIO = new RegistrationReprocessingIODto()
                    }
                }
            }
        };

        var model = new ReprocessedMaterialOutputSummaryModel
        {
            SentToOtherSiteTonnes = 10,
            ContaminantTonnes = 5,
            ProcessLossTonnes = 2,
            ReprocessedMaterialsRawData =new List<ReprocessedMaterialRawDataModel>()
        };

        _sessionManagerMock.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);
        _validationServiceMock.Setup(v => v.ValidateAsync(model, CancellationToken.None))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _registrationMaterialServiceMock.Setup(r => r.UpsertRegistrationReprocessingDetailsAsync(
            It.IsAny<Guid>(),
            It.IsAny<RegistrationReprocessingIODto>()
        )).Returns(Task.CompletedTask);

        var buttonAction = "SaveAndComeBackLater";

        // Act
        var result = await _controller.ReprocessingOutputsForLastYear(model, buttonAction);

        using (new AssertionScope())
        {
            // Assert
            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(PagePaths.ApplicationSaved, redirectResult.Url);
        }
    }

    [TestMethod]
    public async Task ReprocessingOutputsForLastYear_Post_WhenValidationFails_ShouldReturnViewWithModel()
    {
        // Arrange
        var session = new ReprocessorRegistrationSession
        {
            RegistrationId = Guid.NewGuid(),
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingInputsAndOutputs = new ReprocessingInputsAndOutputs
                {
                    CurrentMaterial = new RegistrationMaterialDto
                    {
                        Id = Guid.NewGuid(),
                        MaterialLookup = new MaterialLookupDto { Name = Material.Plastic },
                        RegistrationReprocessingIO = new RegistrationReprocessingIODto()
                    }
                }
            }
        };

        var model = new ReprocessedMaterialOutputSummaryModel
        {
            SentToOtherSiteTonnes = null, // Triggering validation failure
            ContaminantTonnes = 5,
            ProcessLossTonnes = 2,
            ReprocessedMaterialsRawData = new List<ReprocessedMaterialRawDataModel>()
        };

        _sessionManagerMock.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);
        _validationServiceMock.Setup(v => v.ValidateAsync(model, CancellationToken.None))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]
            {
            new FluentValidation.Results.ValidationFailure("SentToOtherSiteTonnes", "Required")
            }));

        // Act
        var result = await _controller.ReprocessingOutputsForLastYear(model, "SaveAndContinue");

        using (new AssertionScope())
        {
            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var returnedModel = viewResult.Model as ReprocessedMaterialOutputSummaryModel;
            Assert.IsNotNull(returnedModel);
            Assert.AreEqual(model, returnedModel); // Should return same model
        }
    }
    [TestMethod]
    public async Task ReprocessingOutputsForLastYear_Post_SessionIsNull_ShouldRedirectToTaskList()
    {
        // Arrange
        _sessionManagerMock.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync((ReprocessorRegistrationSession)null);

        var model = new ReprocessedMaterialOutputSummaryModel();

        // Act
        var result = await _controller.ReprocessingOutputsForLastYear(model, "SaveAndContinue");

        using (new AssertionScope())
        {
            // Assert
            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(PagePaths.TaskList, redirectResult.Url);
        }
    }


}