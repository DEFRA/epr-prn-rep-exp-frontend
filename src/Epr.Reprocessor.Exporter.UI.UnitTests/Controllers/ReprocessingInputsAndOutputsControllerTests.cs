using EPR.Common.Authorization.Extensions;
using Epr.Reprocessor.Exporter.UI.App.Extensions;
using Organisation = EPR.Common.Authorization.Models.Organisation;
using Epr.Reprocessor.Exporter.UI.Sessions;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers;

[TestClass]
public class ReprocessingInputsAndOutputsControllerTests
{
	private Mock<ISessionManager<ReprocessorRegistrationSession>> _sessionManagerMock;
	private Mock<IReprocessorService> _reprocessorServiceMock;
	private Mock<IPostcodeLookupService> _postcodeLookupServiceMock;
	private Mock<IValidationService> _validationServiceMock;
	private Mock<IStringLocalizer<SelectAuthorisationType>> _localizerMock;
	private Mock<IRequestMapper> _requestMapperMock;
	private Mock<HttpContext> _httpContextMock;
	private Mock<IRegistrationMaterialService> _registrationMaterialServiceMock;
    private Mock<IAccountServiceApiClient> _accountServiceMock;
    private ReprocessingInputsAndOutputs _reprocessingInputsAndOutputsSession;

    private ReprocessingInputsAndOutputsController _controller;
    
    [TestInitialize]
	public void SetUp()
	{
		_sessionManagerMock = new Mock<ISessionManager<ReprocessorRegistrationSession>>();
		_reprocessorServiceMock = new Mock<IReprocessorService>();
		_postcodeLookupServiceMock = new Mock<IPostcodeLookupService>();
		_validationServiceMock = new Mock<IValidationService>();
		_localizerMock = new Mock<IStringLocalizer<SelectAuthorisationType>>();
		_requestMapperMock = new Mock<IRequestMapper>();
        _registrationMaterialServiceMock = new Mock<IRegistrationMaterialService>();
        _accountServiceMock = new Mock<IAccountServiceApiClient>();

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
			_localizerMock.Object,
			_requestMapperMock.Object
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
        new RegistrationMaterialDto { MaterialLookup = new MaterialLookupDto { Name = MaterialItem.Plastic } },
        new RegistrationMaterialDto { MaterialLookup = new MaterialLookupDto { Name = MaterialItem.Wood } }
		};

        _reprocessorServiceMock.Setup(rs => rs.RegistrationMaterials.GetAllRegistrationMaterialsAsync(It.IsAny<Guid>()))
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
        new RegistrationMaterialDto { MaterialLookup = new MaterialLookupDto { Name = MaterialItem.Plastic } },
        new RegistrationMaterialDto { MaterialLookup = new MaterialLookupDto { Name = MaterialItem.Wood } }
        };

        _reprocessorServiceMock.Setup(rs => rs.RegistrationMaterials.GetAllRegistrationMaterialsAsync(It.IsAny<Guid>()))
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
            new RegistrationMaterialDto { MaterialLookup = new MaterialLookupDto { Name = MaterialItem.Plastic } }
        };

        _reprocessorServiceMock.Setup(rs => rs.RegistrationMaterials.GetAllRegistrationMaterialsAsync(It.IsAny<Guid>()))
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
			new RegistrationMaterialDto { MaterialLookup = new MaterialLookupDto { Name = MaterialItem.Plastic } }
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
		_reprocessorServiceMock.Setup(rs => rs.RegistrationMaterials.GetAllRegistrationMaterialsAsync(It.IsAny<Guid>()))
			.ReturnsAsync(new List<RegistrationMaterialDto>
			{
			new RegistrationMaterialDto { MaterialLookup = new MaterialLookupDto { Name = MaterialItem.Plastic } }
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
					new RegistrationMaterialDto { MaterialLookup = new MaterialLookupDto { Name = MaterialItem.Plastic }, IsMaterialBeingAppliedFor = true }
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
		_reprocessorServiceMock.Setup(rs => rs.RegistrationMaterials.GetAllRegistrationMaterialsAsync(It.IsAny<Guid>()))
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
		_reprocessorServiceMock.Setup(rs => rs.RegistrationMaterials.GetAllRegistrationMaterialsAsync(It.IsAny<Guid>()))
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
						MaterialLookup = new MaterialLookupDto { Name = MaterialItem.Plastic }
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

		_reprocessorServiceMock.Setup(rs => rs.RegistrationMaterials.GetAllRegistrationMaterialsAsync(It.IsAny<Guid>()))
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
        _reprocessingInputsAndOutputsSession.Materials = [ new RegistrationMaterialDto() ];

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
            MaterialLookup = new MaterialLookupDto { Name = MaterialItem.Plastic },
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
        model.MaterialName.Equals(MaterialItem.Plastic.GetDisplayName());
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
            MaterialLookup = new MaterialLookupDto { Name = MaterialItem.Plastic },
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
        model.MaterialName.Equals(MaterialItem.Plastic.GetDisplayName());
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
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = (RedirectResult)result;
        redirectResult.Url.Should().Be(PagePaths.ApplicationSaved);
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

}