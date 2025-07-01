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
	private Mock<ISession> _sessionMock;
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

		_httpContextMock = new Mock<HttpContext>();
		_sessionMock = new Mock<ISession>();
		_httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);

		_controller = new ReprocessingInputsAndOutputsController(
			_sessionManagerMock.Object,
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
			new RegistrationMaterialDto { MaterialLookup = new MaterialLookupDto { Name = MaterialItem.Plastic } }
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
		Assert.AreEqual(1, model.Materials.Count); 
	}

	[TestMethod]
	public async Task PackagingWasteWillReprocess_Get_WhenSessionIsNull_ShouldCreateNewSessionAndReturnView()
	{
		// Arrange
		_sessionManagerMock.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>()))
			.ReturnsAsync((ReprocessorRegistrationSession)null); 

		var registrationMaterials = new List<RegistrationMaterialDto>
	{
		new RegistrationMaterialDto { MaterialLookup = new MaterialLookupDto { Name = MaterialItem.Plastic } }
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
		Assert.AreEqual(1, model.Materials.Count);
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

}