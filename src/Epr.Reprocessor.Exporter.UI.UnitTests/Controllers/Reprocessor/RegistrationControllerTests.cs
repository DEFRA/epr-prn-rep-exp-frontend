using Epr.Reprocessor.Exporter.UI.App.DTOs.AddressLookup;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;
using Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;
using Epr.Reprocessor.Exporter.UI.App.Enums.Registration;
using Epr.Reprocessor.Exporter.UI.App.Extensions;
using Epr.Reprocessor.Exporter.UI.App.Helpers;
using static Epr.Reprocessor.Exporter.UI.App.Constants.Endpoints;
using Address = Epr.Reprocessor.Exporter.UI.App.Domain.Address;
using TaskStatus = Epr.Reprocessor.Exporter.UI.App.Enums.TaskStatus;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers.Reprocessor;

[TestClass]
public class RegistrationControllerTests
{
    private RegistrationController _controller = null!;
    private Mock<ILogger<RegistrationController>> _logger = null!;
    private Mock<ISaveAndContinueService> _userJourneySaveAndContinueService = null!;
    private Mock<IReprocessorService> _reprocessorService = null!;
    private Mock<IPostcodeLookupService> _postcodeLookupService = null!;
    private Mock<IMaterialService> _mockMaterialService = null!;
    private Mock<IRegistrationMaterialService> _mockRegistrationMaterialService = null!;
    private Mock<IValidationService> _validationService = null!;
    private Mock<IRegistrationService> _registrationService = null!;
    private Mock<IRegistrationMaterialService> _registrationMaterialService = null!;
    private Mock<ISessionManager<ReprocessorRegistrationSession>> _sessionManagerMock = null!;
    private Mock<IRequestMapper> _requestMapper = null!;
    private readonly Mock<HttpContext> _httpContextMock = new();
    private readonly Mock<ClaimsPrincipal> _userMock = new();
    private ReprocessorRegistrationSession _session = null!;
    private Mock<IStringLocalizer<RegistrationController>> _mockLocalizer = new();
    protected ITempDataDictionary TempDataDictionary = null!;

    [TestInitialize]
    public void Setup()
    {
        // ResourcesPath should be 'Resources' but build environment differs from development environment
        // Work around = set ResourcesPath to non-existent location and test for resource keys rather than resource values
        var options = Options.Create(new LocalizationOptions { ResourcesPath = "Resources_not_found" });
        var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
        var localizer = new StringLocalizer<SelectAuthorisationType>(factory);

        _logger = new Mock<ILogger<RegistrationController>>();
        _userJourneySaveAndContinueService = new Mock<ISaveAndContinueService>();
        _sessionManagerMock = new Mock<ISessionManager<ReprocessorRegistrationSession>>();
        _reprocessorService = new Mock<IReprocessorService>();
        _postcodeLookupService = new Mock<IPostcodeLookupService>();
        _mockMaterialService = new Mock<IMaterialService>();
        _mockRegistrationMaterialService = new Mock<IRegistrationMaterialService>();
        _validationService = new Mock<IValidationService>();
        _requestMapper = new Mock<IRequestMapper>();

        _controller = new RegistrationController(_logger.Object, _sessionManagerMock.Object, _reprocessorService.Object, _postcodeLookupService.Object, _validationService.Object, localizer, _requestMapper.Object);

        SetupDefaultUserAndSessionMocks();
        SetupMockPostcodeLookup();

        _registrationService = new Mock<IRegistrationService>();
        _registrationMaterialService = new Mock<IRegistrationMaterialService>();
        _reprocessorService.Setup(o => o.Registrations).Returns(_registrationService.Object);
        _reprocessorService.Setup(o => o.RegistrationMaterials).Returns(_registrationMaterialService.Object);
        _reprocessorService.Setup(o => o.Materials).Returns(_mockMaterialService.Object);

        TempDataDictionary = new TempDataDictionary(_httpContextMock.Object, new Mock<ITempDataProvider>().Object);
        _controller.TempData = TempDataDictionary;
    }

    [TestMethod]
    public async Task ExemptionReferences_Get_ShouldReturnViewWithModel()
    {

        var result = await _controller.ExemptionReferences() as ViewResult;
        var model = result!.Model as ExemptionReferencesViewModel;

        result.Should().BeOfType<ViewResult>();

        model.Should().NotBeNull();
    }

    [Ignore("Logic in code is temp will be removed once the registrationmaterialid is set in the session")]
    [TestMethod]
    public async Task ExemptionReferences_Post_NoErrors_SaveAndContinue_RedirectsToMaximumWeightSiteCanProcess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var model = new ExemptionReferencesViewModel
        {
            ExemptionReferences1 = "EX123456",
            ExemptionReferences2 = "EX654321",
            ExemptionReferences3 = "EX987654",
            ExemptionReferences4 = "EX456789",
            ExemptionReferences5 = "EX321654",

        };

        var materials = new List<RegistrationMaterialDto>
        {
           new ()
           {
               MaterialLookup = new MaterialLookupDto
               {
                   Name = MaterialItem.Steel
               }
           },
           new()
           {
               MaterialLookup = new MaterialLookupDto
               {
                   Name = MaterialItem.Aluminium
               }
           }
        };

        var session = new ReprocessorRegistrationSession
        {
            RegistrationId = id,
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                WasteDetails = new PackagingWaste()
            }
        };

        session.RegistrationApplicationSession.WasteDetails.SetFromExisting(materials);

        //Expectations
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // Act
        var result = await _controller.ExemptionReferences(model, "SaveAndContinue") as RedirectResult;

        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Url.Should().Be(PagePaths.MaximumWeightSiteCanReprocess);
        _mockRegistrationMaterialService.Setup(x => x.CreateExemptionReferences(It.IsAny<CreateExemptionReferencesDto>()))
            .Verifiable();
    }

    [TestMethod]
    [DataRow("", "Enter at least one exemption reference")]
    [DataRow("  ", "Enter at least one exemption reference")]
    [DataRow("testtestdfsffsdsdfsddddddffffffffffffff", "Reference number must not exceed 20 characters")]
    [DataRow("test%&^", "Reference number must include letters, numbers  or '/' only")]
    public async Task Exemptions_Post_ModelErrors_SaveAndContinueShowsErrors_OnSamePage(string inputValue, string errorMessage)
    {         // Arrange
        var model = new ExemptionReferencesViewModel
        {
            ExemptionReferences1 = inputValue,
            ExemptionReferences2 = "",
            ExemptionReferences3 = "",
            ExemptionReferences4 = "",
            ExemptionReferences5 = "",
        };
        _controller.ModelState.AddModelError(string.Empty, errorMessage);

        // Act
        var result = await _controller.ExemptionReferences(model, "SaveAndContinue") as ViewResult;

        // Assert
        result.Should().BeOfType<ViewResult>();
        result.ViewData.ModelState.IsValid.Should().BeFalse();
        Assert.AreEqual(errorMessage, result.ViewData.ModelState.ToDictionary().FirstOrDefault().Value!.Errors.FirstOrDefault()!.ErrorMessage);
    }

    [TestMethod]
    public async Task Exemptions_Post_ModelErrors_Same_Input_SaveAndContinueShowsErrors_OnSamePage()
    {
        // Arrange
        var model = new ExemptionReferencesViewModel
        {
            ExemptionReferences1 = "test",
            ExemptionReferences2 = "test",
            ExemptionReferences3 = "",
            ExemptionReferences4 = "",
            ExemptionReferences5 = "",
        };
        _controller.ModelState.AddModelError(string.Empty, "Exemption reference number already added");

        // Act
        var result = await _controller.ExemptionReferences(model, "SaveAndContinue") as ViewResult;

        // Assert
        result.Should().BeOfType<ViewResult>();
        result.ViewData.ModelState.IsValid.Should().BeFalse();
        Assert.AreEqual("Exemption reference number already added", result.ViewData.ModelState.ToDictionary().FirstOrDefault().Value.Errors.FirstOrDefault().ErrorMessage);
    }

    [Ignore("Logic in code is temp will be removed once the registrationmaterialid is set in the session")]
    [TestMethod]
    public async Task ExemptionReferences_Post_NoErrors_SaveAndComeBackLater_RedirectsToApplicationSaved()
    {
        // Arrange
        var id = Guid.NewGuid();
        var model = new ExemptionReferencesViewModel
        {
            ExemptionReferences1 = "EX123456",
            ExemptionReferences2 = "EX654321",
            ExemptionReferences3 = "EX987654",
            ExemptionReferences4 = "EX456789",
            ExemptionReferences5 = "EX321654",
        };

        var materials = new List<RegistrationMaterialDto>
        {
            new ()
            {
                MaterialLookup = new MaterialLookupDto
                {
                    Name = MaterialItem.Steel
                }
            },
            new()
            {
                MaterialLookup = new MaterialLookupDto
                {
                    Name = MaterialItem.Aluminium
                }
            }
        };

        var session = new ReprocessorRegistrationSession
        {
            RegistrationId = id,
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                WasteDetails = new PackagingWaste()
            }
        };

        session.RegistrationApplicationSession.WasteDetails.SetFromExisting(materials);


        // Expectations
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // Act
        var result = await _controller.ExemptionReferences(model, "SaveAndComeBackLater") as RedirectResult;

        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Url.Should().Be(PagePaths.ApplicationSaved);
    }

    [TestMethod]
    public async Task PpcPermit_Get_ShouldReturnViewWithModel()
    {
        // Arrange
        var result = await _controller.PpcPermit() as ViewResult;
        var model = result!.Model as MaterialPermitViewModel;

        // Act
        result.Should().BeOfType<ViewResult>();

        // Assert
        model.Should().NotBeNull();
    }

    [TestMethod]
    public async Task PpcPermit_Post_NoErrors_ShouldSaveAndGoToNextPage()
    {
        // Arrange
        var model = new MaterialPermitViewModel
        {
            MaximumWeight = "10",
            SelectedFrequency = MaterialFrequencyOptions.PerWeek
        };

        // Expectations
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);
        _userJourneySaveAndContinueService.Setup(x => x.GetLatestAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new SaveAndContinueResponseDto
        {
            Action = nameof(RegistrationController.PpcPermit),
            Controller = nameof(RegistrationController),
            Area = SaveAndContinueAreas.Registration,
            CreatedOn = DateTime.UtcNow,
            Id = 1,
            RegistrationId = 1,
            Parameters = JsonConvert.SerializeObject(model)
        });

        // Act
        var result = await _controller.PpcPermit(model, "SaveAndContinue") as RedirectResult;

        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Url.Should().BeEquivalentTo("placeholder");
    }

    [TestMethod]
    public async Task PpcPermit_Post_NoErrors_SaveComeBackLater_ShouldSaveAndGoToApplicationSavedPage()
    {
        // Arrange
        var model = new MaterialPermitViewModel
        {
            MaximumWeight = "10",
            SelectedFrequency = MaterialFrequencyOptions.PerWeek
        };

        // Expectations
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);
        _userJourneySaveAndContinueService.Setup(x => x.GetLatestAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new SaveAndContinueResponseDto
        {
            Action = nameof(RegistrationController.PpcPermit),
            Controller = nameof(RegistrationController),
            Area = SaveAndContinueAreas.Registration,
            CreatedOn = DateTime.UtcNow,
            Id = 1,
            RegistrationId = 1,
            Parameters = JsonConvert.SerializeObject(model)
        });

        // Act
        var result = await _controller.PpcPermit(model, "SaveAndComeBackLater") as RedirectResult;

        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Url.Should().BeEquivalentTo("application-saved");
    }

    [TestMethod]
    public async Task PpcPermit_Post_ModelErrors_ShouldSaveAndGoToNextPage()
    {
        // Arrange
        var model = new MaterialPermitViewModel
        {
            MaximumWeight = "10",
            SelectedFrequency = MaterialFrequencyOptions.PerWeek
        };
        _controller.ModelState.AddModelError(string.Empty, "error");

        // Act
        var result = await _controller.PpcPermit(model, "SaveAndContinue") as ViewResult;

        // Assert
        result.Should().BeOfType<ViewResult>();
        result.ViewData.ModelState.IsValid.Should().BeFalse();
        var backLinkText = _controller.ViewBag.BackLinkToDisplay as string;
        backLinkText.Should().BeEquivalentTo("permit-for-recycling-waste");
    }

    [TestMethod]
    public async Task MaximumWeightSiteCanReprocess_Get_ShouldReturnViewWithModel()
    {
        // Arrange
        var result = await _controller.MaximumWeightSiteCanReprocess() as ViewResult;
        var model = result!.Model as MaximumWeightSiteCanReprocessViewModel;

        // Act
        result.Should().BeOfType<ViewResult>();

        // Assert
        model.Should().NotBeNull();
    }

    [TestMethod]
    public async Task MaximumWeightSiteCanReprocess_Post_NoErrors_ShouldSaveAndGoToNextPage()
    {
        // Arrange
        var model = new MaximumWeightSiteCanReprocessViewModel
        {
            MaximumWeight = "10",
            SelectedFrequency = MaterialFrequencyOptions.PerWeek
        };

        // Expectations
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);
        _userJourneySaveAndContinueService.Setup(x => x.GetLatestAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new SaveAndContinueResponseDto
        {
            Action = nameof(RegistrationController.MaximumWeightSiteCanReprocess),
            Controller = nameof(RegistrationController),
            Area = SaveAndContinueAreas.Registration,
            CreatedOn = DateTime.UtcNow,
            Id = 1,
            RegistrationId = 1,
            Parameters = JsonConvert.SerializeObject(model)
        });

        // Act
        var result = await _controller.MaximumWeightSiteCanReprocess(model, "SaveAndContinue") as RedirectResult;

        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Url.Should().BeEquivalentTo("placeholder");
        var backLinkText = _controller.ViewBag.BackLinkToDisplay as string;
        backLinkText.Should().BeEquivalentTo("permit-for-recycling-waste");
    }

    [TestMethod]
    public async Task MaximumWeightSiteCanReprocess_Post_NoErrors_SaveComeBackLater_ShouldSaveAndGoToApplicationSavedPage()
    {
        // Arrange
        var model = new MaximumWeightSiteCanReprocessViewModel
        {
            MaximumWeight = "10",
            SelectedFrequency = MaterialFrequencyOptions.PerWeek
        };

        // Expectations
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);
        _userJourneySaveAndContinueService.Setup(x => x.GetLatestAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new SaveAndContinueResponseDto
        {
            Action = nameof(RegistrationController.MaximumWeightSiteCanReprocess),
            Controller = nameof(RegistrationController),
            Area = SaveAndContinueAreas.Registration,
            CreatedOn = DateTime.UtcNow,
            Id = 1,
            RegistrationId = 1,
            Parameters = JsonConvert.SerializeObject(model)
        });

        // Act
        var result = await _controller.MaximumWeightSiteCanReprocess(model, "SaveAndComeBackLater") as RedirectResult;

        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Url.Should().BeEquivalentTo("application-saved");
    }

    [TestMethod]
    public async Task MaximumWeightSiteCanReprocess_Post_ModelErrors_ShouldSaveAndGoToNextPage()
    {
        // Arrange
        var model = new MaximumWeightSiteCanReprocessViewModel
        {
            MaximumWeight = "10",
            SelectedFrequency = MaterialFrequencyOptions.PerWeek
        };
        _controller.ModelState.AddModelError(string.Empty, "error");

        // Act
        var result = await _controller.MaximumWeightSiteCanReprocess(model, "SaveAndContinue") as ViewResult;

        // Assert
        result.Should().BeOfType<ViewResult>();
        result.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [TestMethod]
    public async Task InstallationPermit_Get_ShouldReturnViewWithModel()
    {
        // Arrange
        var result = await _controller.InstallationPermit() as ViewResult;
        var model = result!.Model as MaterialPermitViewModel;

        // Act
        result.Should().BeOfType<ViewResult>();

        // Assert
        model.Should().NotBeNull();
    }

    [TestMethod]
    public async Task InstallationPermit_Post_NoErrors_ShouldSaveAndGoToNextPage()
    {
        // Arrange
        var model = new MaterialPermitViewModel
        {
            MaximumWeight = "10",
            SelectedFrequency = MaterialFrequencyOptions.PerWeek
        };

        // Expectations
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);
        _userJourneySaveAndContinueService.Setup(x => x.GetLatestAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new SaveAndContinueResponseDto
        {
            Action = nameof(RegistrationController.InstallationPermit),
            Controller = nameof(RegistrationController),
            Area = SaveAndContinueAreas.Registration,
            CreatedOn = DateTime.UtcNow,
            Id = 1,
            RegistrationId = 1,
            Parameters = JsonConvert.SerializeObject(model)
        });

        // Act
        var result = await _controller.InstallationPermit(model, "SaveAndContinue") as RedirectResult;

        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Url.Should().BeEquivalentTo("placeholder");
    }

    [TestMethod]
    public async Task InstallationPermit_Post_NoErrors_SaveComeBackLater_ShouldSaveAndGoToApplicationSavedPage()
    {
        // Arrange
        var model = new MaterialPermitViewModel
        {
            MaximumWeight = "10",
            SelectedFrequency = MaterialFrequencyOptions.PerWeek
        };

        // Expectations
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);
        _userJourneySaveAndContinueService.Setup(x => x.GetLatestAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new SaveAndContinueResponseDto
        {
            Action = nameof(RegistrationController.InstallationPermit),
            Controller = nameof(RegistrationController),
            Area = SaveAndContinueAreas.Registration,
            CreatedOn = DateTime.UtcNow,
            Id = 1,
            RegistrationId = 1,
            Parameters = JsonConvert.SerializeObject(model)
        });

        // Act
        var result = await _controller.InstallationPermit(model, "SaveAndComeBackLater") as RedirectResult;

        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Url.Should().BeEquivalentTo("application-saved");
    }

    [TestMethod]
    public async Task InstallationPermit_Post_ModelErrors_ShouldSaveAndGoToNextPage()
    {
        // Arrange
        var model = new MaterialPermitViewModel
        {
            MaximumWeight = "10",
            SelectedFrequency = MaterialFrequencyOptions.PerWeek
        };
        _controller.ModelState.AddModelError(string.Empty, "error");

        // Act
        var result = await _controller.InstallationPermit(model, "SaveAndContinue") as ViewResult;

        // Assert
        result.Should().BeOfType<ViewResult>();
        result.ViewData.ModelState.IsValid.Should().BeFalse();
        string backLinkText = _controller.ViewBag.BackLinkToDisplay;
        backLinkText.Should().BeEquivalentTo("permit-for-recycling-waste");
    }

    [TestMethod]
    public async Task TaskList_Get_ReturnsExpectedTaskListModel()
    {
        // Arrange
        var session = new ReprocessorRegistrationSession();
        var expectedTaskListInModel = new List<TaskItem>
        {
            new(){TaskName = TaskType.SiteAndContactDetails, Url = "address-of-reprocessing-site", Status = TaskStatus.NotStart},
            new(){TaskName = TaskType.WasteLicensesPermitsExemptions, Url = "select-materials-authorised-to-recycle", Status = TaskStatus.CannotStartYet},
            new(){TaskName = TaskType.ReprocessingInputsOutputs, Url = "#", Status = TaskStatus.CannotStartYet},
            new(){TaskName = TaskType.SamplingAndInspectionPlan, Url = "#", Status = TaskStatus.CannotStartYet}
        };

        // Expectations
        _sessionManagerMock.Setup(o => o.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // Act
        var result = await _controller.TaskList() as ViewResult;

        // Assert
        Assert.IsNotNull(result, "Result should not be null");
        var model = result.Model as TaskListModel;
        model!.TaskList.Should().BeEquivalentTo(expectedTaskListInModel);
        model.HaveAllBeenCompleted.Should().BeFalse();
    }

    [TestMethod]
    public async Task WastePermitExemptions_Get_ReturnsViewWithModel()
    {
        // Arrange
        var mockFactory = new Mock<IModelFactory<WastePermitExemptionsViewModel>>();
        var registrationId = Guid.NewGuid();
        var session = new ReprocessorRegistrationSession
        {
            RegistrationId = registrationId,
            RegistrationApplicationSession = new()
            {
                WasteDetails = new()
                {
                    SelectedMaterials = [new() { Name = MaterialItem.Aluminium }]
                }
            }
        };

        var materials = new List<MaterialLookupDto>
        {
            new() { Code = "AL", Name = MaterialItem.Aluminium },
            new() { Code = "PL", Name = MaterialItem.Plastic }
        };

        var registrationMaterialDtos = new List<RegistrationMaterialDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                RegistrationId = registrationId,
                MaterialLookup = new MaterialLookupDto
                {
                    Name = MaterialItem.Aluminium
                }
            }
        };

        // Expectations
        mockFactory.Setup(o => o.Instance).Returns(new WastePermitExemptionsViewModel());

        _mockMaterialService.Setup(o => o.GetAllMaterialsAsync()).ReturnsAsync(materials);
        _sessionManagerMock.Setup(o => o.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);
        _registrationMaterialService.Setup(o => o.GetAllRegistrationMaterialsAsync(registrationId))
            .ReturnsAsync(registrationMaterialDtos);

        // Act
        var result = await _controller.WastePermitExemptions(mockFactory.Object);

        // Assert
        result.Should().BeOfType<ViewResult>();
        result.Should().NotBeNull();
    }

    [TestMethod]
    public async Task WastePermitExemptions_Get_NoRegistrationId_Retrieve_NullRegistration_Redirect()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var userData = NewUserData.Build();

        var mockFactory = new Mock<IModelFactory<WastePermitExemptionsViewModel>>();
        var session = new ReprocessorRegistrationSession
        {
            RegistrationApplicationSession = new()
            {
                WasteDetails = new()
                {
                    SelectedMaterials = [new() { Name = MaterialItem.Aluminium }]
                }
            }
        };

        // Expectations
        mockFactory.Setup(o => o.Instance).Returns(new WastePermitExemptionsViewModel());
        _registrationService.Setup(o => o.GetByOrganisationAsync(1, organisationId))
            .ReturnsAsync((RegistrationDto?)null);

        _sessionManagerMock.Setup(o => o.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);
        SetupMockHttpContext(CreateClaims(userData));

        // Act
        var result = await _controller.WastePermitExemptions(mockFactory.Object);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        (result as RedirectResult)!.Url.Should().BeEquivalentTo("reprocessor-registration-task-list");
        result.Should().NotBeNull();
    }

    [TestMethod]
    public async Task WastePermitExemptions_Get_NoRegistrationId_Retrieve_RegistrationExists_WithExistingWasteDetailMaterials()
    {
        // Arrange
        var registrationId = Guid.NewGuid();
        var model = new WastePermitExemptionsViewModel();
        var userData = NewUserData.Build();

        var mockFactory = new Mock<IModelFactory<WastePermitExemptionsViewModel>>();
        var session = new ReprocessorRegistrationSession
        {
            RegistrationApplicationSession = new()
            {
                WasteDetails = new()
                {
                    AllMaterials = [new() { Name = MaterialItem.Steel }]
                }
            }
        };

        var registrationMaterialDtos = new List<RegistrationMaterialDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                RegistrationId = registrationId,
                MaterialLookup = new MaterialLookupDto
                {
                    Name = MaterialItem.Steel
                }
            }
        };

        // Expectations
        mockFactory.Setup(o => o.Instance).Returns(model);
        _registrationService.Setup(o => o.GetByOrganisationAsync(1, userData.Organisations.First().Id!.Value))
        .ReturnsAsync(new RegistrationDto
        {
                 Id = Guid.NewGuid()
        });

        _registrationMaterialService.Setup(o => o.GetAllRegistrationMaterialsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(registrationMaterialDtos);

        _mockMaterialService.Setup(o => o.GetAllMaterialsAsync()).ReturnsAsync(new List<MaterialLookupDto>
        {
            new()
            {
                Id = 1,
                Name = MaterialItem.Steel,
                Code = "ST"
            }
        });

        _sessionManagerMock.Setup(o => o.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);
        SetupMockHttpContext(CreateClaims(userData));

        // Act
        var result = await _controller.WastePermitExemptions(mockFactory.Object);

        // Assert
        result.Should().BeOfType<ViewResult>();
        result.Should().NotBeNull();
        model.Materials.Should().BeEquivalentTo(new List<CheckboxItem>
        {
            new()
            {
                LabelText = "Steel (R4)",
                Value = "Steel",
                IsChecked = true
            }
        });
    }

    [TestMethod]
    public async Task WastePermitExemptions_Post_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        var model = new WastePermitExemptionsViewModel();

        // Expectations
        _mockMaterialService.Setup(o => o.GetAllMaterialsAsync()).ReturnsAsync(new List<MaterialLookupDto>
        {
            new()
            {
                Name = MaterialItem.Steel
            }
        });

        _controller.ModelState.AddModelError("SelectedMaterials", "error");

        // Act
        var result = await _controller.WastePermitExemptions(model, "SaveAndContinue");

        // Assert
        result.Should().BeOfType<ViewResult>();
        result.Should().NotBeNull();
        model.Materials.Should().BeEquivalentTo(new List<CheckboxItem>
        {
            new()
            {
                Value = nameof(MaterialItem.Steel),
                LabelText = "Steel (R4)"
            }
        });
    }

    [TestMethod]
    public async Task AddressForNotices_Get_ShouldReturnView()
    {
        var ReprocessorRegistrationSession = CreateReprocessorRegistrationSession();
        var userData = GetUserDateWithNationIdAndCompanyNumber();

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(ReprocessorRegistrationSession);

        var claims = CreateClaims(userData);

        _userMock.Setup(x => x.Claims).Returns(claims);
        _httpContextMock.Setup(x => x.User).Returns(_userMock.Object);
        _controller.ControllerContext.HttpContext = _httpContextMock.Object;

        // Act
        var result = await _controller.AddressForNotices();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [TestMethod]
    [DataRow(PagePaths.GridReferenceForEnteredReprocessingSite, PagePaths.GridReferenceForEnteredReprocessingSite)]
    [DataRow(PagePaths.GridReferenceOfReprocessingSite, PagePaths.GridReferenceOfReprocessingSite)]
    [DataRow(PagePaths.ManualAddressForReprocessingSite, PagePaths.ManualAddressForReprocessingSite)]
    public async Task AddressForNotices_Get_ShouldSetBackLink(string sourcePage, string backLink)
    {
        var ReprocessorRegistrationSession = CreateReprocessorRegistrationSession();
        var userData = GetUserDateWithNationIdAndCompanyNumber();

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(ReprocessorRegistrationSession);

        var claims = CreateClaims(userData);

        _userMock.Setup(x => x.Claims).Returns(claims);
        _httpContextMock.Setup(x => x.User).Returns(_userMock.Object);
        _controller.ControllerContext.HttpContext = _httpContextMock.Object;

        // Act
        var result = await _controller.AddressForNotices() as ViewResult;
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;

        // Assert
        result.Should().BeOfType<ViewResult>();
        backlink.Should().Be(backlink);
    }

    [TestMethod]
    public async Task AddressForNotices_Get_NoNationId_ReturnRedirectResult()
    {
        // Arrange
        var model = new AddressForNoticesViewModel();

        // Expectations
        SetupDefaultUserAndSessionMocks();

        _userJourneySaveAndContinueService.Setup(x => x.GetLatestAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((SaveAndContinueResponseDto?)null!);

        // Act
        var result = await _controller.AddressForNotices() as RedirectResult;

        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Url.Should().BeEquivalentTo("country-of-reprocessing-site");
    }

    [TestMethod]
    [DataRow(AddressOptions.DifferentAddress, true)]
    [DataRow(AddressOptions.BusinessAddress, false)]
    [DataRow(AddressOptions.RegisteredAddress, false)]
    [DataRow(AddressOptions.SiteAddress, false)]

    public async Task AddressForNotices_Get_ReturnsViewWithModel(AddressOptions addressOptions, bool showSiteRadioButton)
    {
        // Arrange
        var organisation = NewOrganisation
            .Set(o => o.BuildingNumber, "10")
            .Set(o => o.Street, "Downing Street")
            .Set(o => o.Locality, "line 2")
            .Set(o => o.Town, "London")
            .Set(o => o.County, "Greater London")
            .Set(o => o.Postcode, "G12 3GX")
            .Build();

        var userData = NewUserData
            .Set(o => o.Organisations, [organisation])
            .Build();

        var registerApplicationSession = new RegistrationApplicationSession
        {
            ReprocessingSite = new ReprocessingSite
            {
                TypeOfAddress = addressOptions,
                SourcePage = PagePaths.GridReferenceOfReprocessingSite,
                Address = new Address("10 Downing Street", "line 2", "London", "London", "Greater London", "UK", "G12 3GX")
            }
        };

        var ReprocessorRegistrationSession = new ReprocessorRegistrationSession
        {
            Journey = new List<string> { PagePaths.GridReferenceOfReprocessingSite, PagePaths.AddressForNotices },
            UserData = userData,
            RegistrationApplicationSession = registerApplicationSession
        };

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(ReprocessorRegistrationSession);

        var claims = CreateClaims(userData);

        _userMock.Setup(x => x.Claims).Returns(claims);
        _httpContextMock.Setup(x => x.User).Returns(_userMock.Object);
        _controller.ControllerContext.HttpContext = _httpContextMock.Object;

        // Act
        var result = await _controller.AddressForNotices() as ViewResult;

        // Assert
        result.Should().BeOfType<ViewResult>();
        result.Should().NotBeNull();
        result.Model.Should().BeOfType<AddressForNoticesViewModel>();
        var addressForNoticeViewModel = result.Model as AddressForNoticesViewModel;
        addressForNoticeViewModel.Should().NotBeNull();

        addressForNoticeViewModel.BusinessAddress.Should().NotBeNull();
        addressForNoticeViewModel.BusinessAddress.AddressLine1.Should().Be("10 Downing Street");
        addressForNoticeViewModel.BusinessAddress.AddressLine2.Should().Be("line 2");
        addressForNoticeViewModel.BusinessAddress.Postcode.Should().Be("G12 3GX");
        addressForNoticeViewModel.BusinessAddress.County.Should().Be("Greater London");
        addressForNoticeViewModel.BusinessAddress.TownOrCity.Should().Be("London");

        addressForNoticeViewModel.SiteAddress.Should().NotBeNull();
        addressForNoticeViewModel.SiteAddress.AddressLine1.Should().Be("10 Downing Street");
        addressForNoticeViewModel.SiteAddress.AddressLine2.Should().Be("line 2");
        addressForNoticeViewModel.SiteAddress.Postcode.Should().Be("G12 3GX");
        addressForNoticeViewModel.SiteAddress.County.Should().Be("Greater London");
        addressForNoticeViewModel.SiteAddress.TownOrCity.Should().Be("London");

        addressForNoticeViewModel.ShowSiteAddress.Should().Be(showSiteRadioButton);
    }

    [TestMethod]
    [DataRow(PagePaths.GridReferenceForEnteredReprocessingSite)]
    [DataRow(PagePaths.GridReferenceOfReprocessingSite)]
    [DataRow(PagePaths.ManualAddressForReprocessingSite)]
    public async Task AddressForNotices_Get_ShouldSaveSession(string sourcePage)
    {
        var ReprocessorRegistrationSession = CreateReprocessorRegistrationSession();
        var userData = GetUserDateWithNationIdAndCompanyNumber();

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(ReprocessorRegistrationSession);

        var claims = CreateClaims(userData);

        _userMock.Setup(x => x.Claims).Returns(claims);
        _httpContextMock.Setup(x => x.User).Returns(_userMock.Object);
        _controller.ControllerContext.HttpContext = _httpContextMock.Object;
        ReprocessorRegistrationSession.RegistrationApplicationSession.ReprocessingSite.SetSourcePage(sourcePage);

        // Act
        var result = await _controller.AddressForNotices() as ViewResult;

        // Assert
        result.Should().BeOfType<ViewResult>();

        _sessionManagerMock.Verify(x => x.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<ReprocessorRegistrationSession>()), Times.Once);

        ReprocessorRegistrationSession.Journey.Should().HaveCount(2);
        ReprocessorRegistrationSession.Journey[0].Should().Be(sourcePage);
        ReprocessorRegistrationSession.Journey[1].Should().Be(PagePaths.AddressForNotices);
    }

    [TestMethod]
    public async Task AddressForNotices_Post_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        var model = new AddressForNoticesViewModel();
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
        var result = await _controller.AddressForNotices(model, "SaveAndContinue");

        // Assert
        result.Should().BeOfType<ViewResult>();
        result.Should().NotBeNull();
        _controller.ModelState["SelectedOption"].Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be("SelectedOption is required");
    }

    [TestMethod]
    [DataRow(AddressOptions.BusinessAddress, PagePaths.CheckAnswers)]
    [DataRow(AddressOptions.RegisteredAddress, PagePaths.CheckAnswers)]
    [DataRow(AddressOptions.SiteAddress, PagePaths.CheckAnswers)]
    [DataRow(AddressOptions.DifferentAddress, PagePaths.PostcodeForServiceOfNotices)]
    public async Task AddressForNotices_Post_ValidModel_NoticesAddress_Selection_NextPage(AddressOptions addressOptions, string nextPage)
    {
        var businessAddress = new AddressViewModel
        {
            AddressLine1 = "10 Downing Street Business Address",
            AddressLine2 = "London",
            Postcode = "G12 3GX",
            County = "Greater London",
            TownOrCity = "London"
        };

        var siteAddress = new AddressViewModel
        {
            AddressLine1 = "10 Downing Street Site Address",
            AddressLine2 = "line 2",
            Postcode = "G12 3GX",
            County = "Greater London",
            TownOrCity = "London"
        };

        var model = new AddressForNoticesViewModel
        {
            SelectedAddressOptions = addressOptions,
            BusinessAddress = businessAddress,
            SiteAddress = siteAddress
        };

        _validationService.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var ReprocessorRegistrationSession = CreateReprocessorRegistrationSession();
        var userData = GetUserDateWithNationIdAndCompanyNumber();

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(ReprocessorRegistrationSession);

        var claims = CreateClaims(userData);

        _userMock.Setup(x => x.Claims).Returns(claims);
        _httpContextMock.Setup(x => x.User).Returns(_userMock.Object);
        _controller.ControllerContext.HttpContext = _httpContextMock.Object;

        // Act
        var result = await _controller.AddressForNotices(model, "SaveAndContinue") as RedirectResult;

        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Url.Should().Be(nextPage);
    }

    [TestMethod]
    [DataRow(AddressOptions.BusinessAddress)]
    [DataRow(AddressOptions.RegisteredAddress)]
    [DataRow(AddressOptions.SiteAddress)]
    public async Task AddressForNotices_Post_ValidModel_ReprocessingSiteAddressToSession(AddressOptions addressOptions)
    {
        var businessAddress = new AddressViewModel
        {
            AddressLine1 = "10 Downing Street Business Address",
            AddressLine2 = "London",
            Postcode = "G12 3GX",
            County = "Greater London",
            TownOrCity = "London"
        };

        var siteAddress = new AddressViewModel
        {
            AddressLine1 = "10 Downing Street Site Address",
            AddressLine2 = "line 2",
            Postcode = "G12 3GX",
            County = "Greater London",
            TownOrCity = "London"
        };

        var model = new AddressForNoticesViewModel
        {
            SelectedAddressOptions = addressOptions,
            BusinessAddress = businessAddress,
            SiteAddress = siteAddress
        };

        _validationService.Setup(v => v.ValidateAsync(model, CancellationToken.None))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var reprocessorRegistrationSession = CreateReprocessorRegistrationSession();
        var userData = GetUserDateWithNationIdAndCompanyNumber();

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(reprocessorRegistrationSession);

        var claims = CreateClaims(userData);

        _userMock.Setup(x => x.Claims).Returns(claims);
        _httpContextMock.Setup(x => x.User).Returns(_userMock.Object);
        _controller.ControllerContext.HttpContext = _httpContextMock.Object;

        // Act
        var result = await _controller.AddressForNotices(model, "SaveAndContinue") as RedirectResult;

        reprocessorRegistrationSession.RegistrationApplicationSession.ReprocessingSite!.ServiceOfNotice!.Address.Should().NotBeNull();
        reprocessorRegistrationSession.RegistrationApplicationSession.ReprocessingSite.ServiceOfNotice.TypeOfAddress.Should().Be(addressOptions);
    }

    [TestMethod]
    public async Task AddressForNotices_Post_ValidModel_DifferentAddress_ReprocessingSiteAddressToSession()
    {
        var businessAddress = new AddressViewModel
        {
            AddressLine1 = "10 Downing Street Business Address",
            AddressLine2 = "London",
            Postcode = "G12 3GX",
            County = "Greater London",
            TownOrCity = "London"
        };

        var siteAddress = new AddressViewModel
        {
            AddressLine1 = "10 Downing Street Site Address",
            AddressLine2 = "line 2",
            Postcode = "G12 3GX",
            County = "Greater London",
            TownOrCity = "London"
        };

        var model = new AddressForNoticesViewModel
        {
            SelectedAddressOptions = AddressOptions.DifferentAddress,
            BusinessAddress = businessAddress,
            SiteAddress = siteAddress
        };

        _validationService.Setup(v => v.ValidateAsync(model, CancellationToken.None))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var reprocessorRegistrationSession = CreateReprocessorRegistrationSession();
        var userData = GetUserDateWithNationIdAndCompanyNumber();

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(reprocessorRegistrationSession);

        var claims = CreateClaims(userData);

        _userMock.Setup(x => x.Claims).Returns(claims);
        _httpContextMock.Setup(x => x.User).Returns(_userMock.Object);
        _controller.ControllerContext.HttpContext = _httpContextMock.Object;

        // Act
        var result = await _controller.AddressForNotices(model, "SaveAndContinue") as RedirectResult;

        reprocessorRegistrationSession.RegistrationApplicationSession.ReprocessingSite!.ServiceOfNotice!.Address.Should().BeNull();
        reprocessorRegistrationSession.RegistrationApplicationSession.ReprocessingSite.ServiceOfNotice.TypeOfAddress.Should().Be(AddressOptions.DifferentAddress);
    }

    [TestMethod]
    public async Task UkSiteLocation_ShouldReturnView()
    {
        _session = new ReprocessorRegistrationSession();
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
        backlink.Should().Be(PagePaths.AddressOfReprocessingSite);
    }

    [TestMethod]
    public async Task UKSiteLocation_Get_ReturnsViewWithModel()
    {
        // Arrange
        var session = new ReprocessorRegistrationSession { Journey = new List<string>() };
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
        var result = await _controller.UKSiteLocation(model);

        // Assert
        result.Should().BeOfType<ViewResult>();
        result.Should().NotBeNull();
    }

    [TestMethod]
    public async Task UkSiteLocation_ShouldSaveSession()
    {
        _session = new ReprocessorRegistrationSession();
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        // Act
        var result = await _controller.UKSiteLocation() as ViewResult;
        var session = _controller.HttpContext.Session as ReprocessorRegistrationSession;
        // Assert
        result.Should().BeOfType<ViewResult>();

        _sessionManagerMock.Verify(x => x.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<ReprocessorRegistrationSession>()), Times.Once);

        _session.Journey.Count.Should().Be(1);
        _session.Journey[0].Should().Be(PagePaths.AddressOfReprocessingSite);
    }

    [TestMethod]
    public async Task UkSiteLocation_OnSubmit_ShouldValidateModel()
    {
        var model = new UKSiteLocationViewModel() { SiteLocationId = null };
        var expectedErrorMessage = "Select the country the reprocessing site is located in.";
        ValidateViewModel(model);

        // Act
        var result = await _controller.UKSiteLocation(model);
        var modelState = _controller.ModelState;

        // Assert
        result.Should().BeOfType<ViewResult>();

        Assert.AreEqual(1, modelState["SiteLocationId"].Errors.Count);
        Assert.AreEqual(expectedErrorMessage, modelState["SiteLocationId"].Errors[0].ErrorMessage);
    }

    [TestMethod]
    [DataRow(UkNation.England)]
    [DataRow(UkNation.None)]
    [DataRow(UkNation.NorthernIreland)]
    [DataRow(UkNation.Wales)]
    [DataRow(UkNation.Scotland)]
    public async Task UKSiteLocation_ReprocessingSiteNation_ModelSiteLocationIdNone(UkNation nation)
    {
        // Arrange  

        var session = new ReprocessorRegistrationSession
        {
            RegistrationApplicationSession = new RegistrationApplicationSession
            {
                ReprocessingSite = new ReprocessingSite { Nation = nation }

            }
        };

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // Act  
        var result = await _controller.UKSiteLocation() as ViewResult;
        var viewModel = result?.Model as UKSiteLocationViewModel;

        // Assert  
        Assert.IsNotNull(viewModel);
        viewModel.SiteLocationId.Should().Be(nation);
    }



    [TestMethod]
    public async Task UkSiteLocation_OnSubmit_SaveAndContinue_ShouldRedirectNextPage()
    {
        var model = new UKSiteLocationViewModel() { SiteLocationId = UkNation.England };

        ValidateViewModel(model);

        // Act
        var result = await _controller.UKSiteLocation(model) as RedirectResult;

        // Assert
        result.Should().BeOfType<RedirectResult>();

        result.Url.Should().Be(PagePaths.PostcodeOfReprocessingSite);
    }

    [TestMethod]
    public async Task UkSiteLocation_OnSubmit_SaveAndContinue_ShouldSetBackLink()
    {
        _session = new ReprocessorRegistrationSession() { Journey = new List<string> { PagePaths.AddressForLegalDocuments, PagePaths.CountryOfReprocessingSite } };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);
        var model = new UKSiteLocationViewModel() { SiteLocationId = UkNation.England };

        ValidateViewModel(model);

        // Act
        var result = await _controller.UKSiteLocation(model) as RedirectResult;
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;
        // Assert
        result.Should().BeOfType<RedirectResult>();

        backlink.Should().Be(PagePaths.AddressForNotices);
    }

    [TestMethod]
    public async Task UkSiteLocation_OnSubmit_SaveAndComeBackLater_ShouldRedirectNextPage()
    {
        var model = new UKSiteLocationViewModel() { SiteLocationId = UkNation.England };
        var expectedModel = JsonConvert.SerializeObject(model);

        // Act
        var result = await _controller.UKSiteLocation(model) as RedirectResult;

        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Url.Should().Be(PagePaths.PostcodeOfReprocessingSite);
    }

    [TestMethod]
    public async Task UkSiteLocation_OnSubmit_SaveAndComeBackLater_ShouldSetBackLink()
    {
        _session = new ReprocessorRegistrationSession() { Journey = new List<string> { PagePaths.AddressForLegalDocuments, PagePaths.CountryOfReprocessingSite } };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        var model = new UKSiteLocationViewModel() { SiteLocationId = UkNation.England };
        var expectedModel = JsonConvert.SerializeObject(model);

        // Act
        var result = await _controller.UKSiteLocation(model) as RedirectResult;
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;
        // Assert
        result.Should().BeOfType<RedirectResult>();
        backlink.Should().Be(PagePaths.AddressForNotices);
    }

    [TestMethod]
    public async Task NoAddressFound_ShouldReturnViewWithModel()
    {
        var result = await _controller.NoAddressFound(AddressLookupType.ReprocessingSite) as ViewResult;
        var model = result!.Model as NoAddressFoundViewModel;

        result.Should().BeOfType<ViewResult>();
        model.Should().NotBeNull();
    }

    [TestMethod]
    public async Task PostcodeOfReprocessingSite_Get_ShouldReturnViewWithModel()
    {
        var result = await _controller.PostcodeOfReprocessingSite() as ViewResult;
        var model = result!.Model as PostcodeOfReprocessingSiteViewModel;

        result.Should().BeOfType<ViewResult>();
        model.Should().NotBeNull();
    }

    [TestMethod]
    public async Task PostcodeOfReprocessingSite_ShouldSetBackLink()
    {
        // Act
        var result = await _controller.PostcodeOfReprocessingSite() as ViewResult;
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;
        // Assert
        result.Should().BeOfType<ViewResult>();
        backlink.Should().Be(PagePaths.CountryOfReprocessingSite);
    }

    [TestMethod]
    public async Task PostcodeOfReprocessingSite_Post_ShouldReturnViewWithModel()
    {
        var model = new PostcodeOfReprocessingSiteViewModel { Postcode = "TA1 2XY" };

        _validationService.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var result = await _controller.PostcodeOfReprocessingSite(model) as RedirectResult;

        result.Should().BeOfType<RedirectResult>();
    }

    [TestMethod]
    public async Task AddressOfReprocessingSite_Get_NoSaveAndContinue_NoNationId_RedirectToCountryOfProcessingSite()
    {
        // Arrange
        var model = new AddressOfReprocessingSiteViewModel();

        // Expectations
        SetupDefaultUserAndSessionMocks();

        _userJourneySaveAndContinueService.Setup(x => x.GetLatestAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((SaveAndContinueResponseDto?)null!);

        // Act
        var result = await _controller.AddressOfReprocessingSite() as RedirectResult;

        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Url.Should().BeEquivalentTo("country-of-reprocessing-site");
    }

    [TestMethod]
    public async Task AddressOfReprocessingSite_Get_NoSaveAndContinue_NotACompany_SetBusinessAddress_ShowView()
    {
        // Arrange
        var expectedModel = new AddressOfReprocessingSiteViewModel
        {
            BusinessAddress = new()
            {
                AddressLine1 = "51 address line 1",
                AddressLine2 = "address line 2",
                Postcode = "CV1 1TT",
                County = "West Midlands",
                TownOrCity = "Birmingham"
            }
        };

        var userData = new UserData
        {
            Id = Guid.NewGuid(),
            Organisations =
            [
                new()
                {
                    Name = "mega limited",
                    NationId = 1,
                    CompaniesHouseNumber = string.Empty,
                    Street = "address line 1",
                    Locality = "address line 2",
                    BuildingNumber = "51",
                    Country = "England",
                    County = "West Midlands",
                    Postcode = "CV1 1TT",
                    Town = "Birmingham"
                }
            ]
        };

        var claims = new List<Claim>
        {
            new(ClaimTypes.UserData, System.Text.Json.JsonSerializer.Serialize(userData))
        };

        // Expectations
        SetupMockSession();
        SetupMockHttpContext(claims);
        _userJourneySaveAndContinueService.Setup(x => x.GetLatestAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((SaveAndContinueResponseDto?)null!);

        // Act
        var result = await _controller.AddressOfReprocessingSite() as ViewResult;

        // Assert
        result!.Model.Should().BeEquivalentTo(expectedModel);
    }

    [TestMethod]
    public async Task AddressOfReprocessingSite_Get_NoSaveAndContinue_IsACompany_SetRegisteredAddress_ShowView()
    {
        // Arrange
        var expectedModel = new AddressOfReprocessingSiteViewModel
        {
            RegisteredAddress = new()
            {
                AddressLine1 = "51 address line 1",
                AddressLine2 = "address line 2",
                Postcode = "CV1 1TT",
                County = "West Midlands",
                TownOrCity = "Birmingham"
            }
        };

        var userData = new UserData
        {
            Id = Guid.NewGuid(),
            Organisations =
            [
                new()
                {
                    Name = "mega limited",
                    NationId = 1,
                    CompaniesHouseNumber = "123456",
                    Street = "address line 1",
                    Locality = "address line 2",
                    BuildingNumber = "51",
                    Country = "England",
                    County = "West Midlands",
                    Postcode = "CV1 1TT",
                    Town = "Birmingham"
                }
            ]
        };

        var claims = new List<Claim>
        {
            new(ClaimTypes.UserData, System.Text.Json.JsonSerializer.Serialize(userData))
        };

        // Expectations
        SetupMockSession();
        SetupMockHttpContext(claims);
        _userJourneySaveAndContinueService.Setup(x => x.GetLatestAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((SaveAndContinueResponseDto?)null!);

        // Act
        var result = await _controller.AddressOfReprocessingSite() as ViewResult;

        // Assert
        result!.Model.Should().BeEquivalentTo(expectedModel);
    }

    [TestMethod]
    [DataRow(AddressOptions.RegisteredAddress, PagePaths.GridReferenceOfReprocessingSite)]
    [DataRow(AddressOptions.BusinessAddress, PagePaths.GridReferenceOfReprocessingSite)]
    [DataRow(AddressOptions.DifferentAddress, PagePaths.CountryOfReprocessingSite)]
    public async Task AddressOfReprocessingSite_Post_ValidModel_SelectedOptionIsRegisteredOrSiteAddress_NavigateToGridReferenceOfReprocessingSite(
        AddressOptions addressOptions, string nextPagePath)
    {
        // Arrange
        var model = new AddressOfReprocessingSiteViewModel
        {
            SelectedOption = addressOptions,
            BusinessAddress = new()
            {
                AddressLine1 = "51 address line 1",
                AddressLine2 = "address line 2",
                Postcode = "CV1 1TT",
                County = "West Midlands",
                TownOrCity = "Birmingham"
            },
            RegisteredAddress = null,
        };

        // Expectations
        _validationService.Setup(v => v.ValidateAsync(model, CancellationToken.None))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // Act
        var result = await _controller.AddressOfReprocessingSite(model) as RedirectResult;

        // Assert
        result.Should().NotBeNull();
        result.Url.Should().Be(nextPagePath);
    }

    [TestMethod]
    public async Task AddressOfReprocessingSite_Post_ValidModel_SelectedOptionIsDifferentAddress_NavigateToCountryOfProcessingSite()
    {
        // Arrange
        var model = new AddressOfReprocessingSiteViewModel
        {
            SelectedOption = AddressOptions.DifferentAddress,
            BusinessAddress = null,
            RegisteredAddress = null,
        };

        // Expectations
        _validationService.Setup(v => v.ValidateAsync(model, CancellationToken.None))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // Act
        var result = await _controller.AddressOfReprocessingSite(model) as RedirectResult;

        // Assert
        result.Should().NotBeNull();
        result.Url.Should().Be("country-of-reprocessing-site");
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
        _session = new ReprocessorRegistrationSession();
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

        backlink.Should().Be(PagePaths.SelectAddressForReprocessingSite);
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

        var modelStateErrorCount = modelState.ContainsKey("GridReference") ? modelState["GridReference"]!.Errors.Count : modelState[""]!.Errors.Count;
        var modelStateErrorMessage = modelState.ContainsKey("GridReference") ? modelState["GridReference"]!.Errors[0].ErrorMessage : modelState[""]!.Errors[0].ErrorMessage;

        Assert.AreEqual(1, modelStateErrorCount);
        Assert.AreEqual(expectedErrorMessage, modelStateErrorMessage);
    }

    [TestMethod]
    [DataRow("TF123434")]
    [DataRow("TF3333")]
    [DataRow("TF3214193478")]
    public async Task ProvideSiteGridReference_OnSubmit_ShouldBeSuccessful(string gridReference)
    {
        // Arrange
        var id = Guid.NewGuid();
        var saveAndContinue = "SaveAndContinue";
        var model = new ProvideSiteGridReferenceViewModel { GridReference = gridReference };
        ValidateViewModel(model);

        var request = new CreateRegistrationDto
        {
            ApplicationTypeId = 1
        };

        var response = new CreateRegistrationResponseDto
        {
            Id = id
        };

        // Expectations
        _requestMapper.Setup(o => o.MapForCreate()).ReturnsAsync(request);

        _registrationService.Setup(o => o.CreateAsync(request)).ReturnsAsync(response);
        _reprocessorService.Setup(o => o.Registrations).Returns(_registrationService.Object);


        // Act
        var result = await _controller.ProvideSiteGridReference(model, saveAndContinue);

        // Assert
        result.Should().BeOfType<RedirectResult>();
    }

    [TestMethod]
    [DataRow("SaveAndContinue", PagePaths.SelectAddressForReprocessingSite)]
    [DataRow("SaveAndComeBackLater", PagePaths.SelectAddressForReprocessingSite)]
    public async Task ProvideSiteGridReference_OnSubmit_ShouldSetBackLink(string actionButton, string backLinkUrl)
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new CreateRegistrationDto
        {
            ApplicationTypeId = 1
        };

        var response = new CreateRegistrationResponseDto
        {
            Id = id
        };

        _session = new ReprocessorRegistrationSession { Journey = new List<string> { PagePaths.SelectAddressForReprocessingSite, PagePaths.GridReferenceForEnteredReprocessingSite } };

        // Expectations
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        _requestMapper.Setup(o => o.MapForCreate()).ReturnsAsync(request);

        _registrationService.Setup(o => o.CreateAsync(request)).ReturnsAsync(response);
        _reprocessorService.Setup(o => o.Registrations).Returns(_registrationService.Object);

        var model = new ProvideSiteGridReferenceViewModel() { GridReference = "1245412545" };

        // Act
        await _controller.ProvideSiteGridReference(model, actionButton);
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;

        // Assert
        backlink.Should().Be(backLinkUrl);
    }

    [TestMethod]
    [DataRow("SaveAndContinue", PagePaths.AddressForNotices)]
    [DataRow("SaveAndComeBackLater", PagePaths.ApplicationSaved)]
    public async Task ProvideSiteGridReference_OnSubmit_ShouldRedirect(string actionButton, string expectedReturnUrl)
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new CreateRegistrationDto
        {
            ApplicationTypeId = 1
        };

        var response = new CreateRegistrationResponseDto
        {
            Id = id
        };
        var model = new ProvideSiteGridReferenceViewModel { GridReference = "1245412545" };
        _session = new ReprocessorRegistrationSession { Journey = new List<string> { "", PagePaths.GridReferenceForEnteredReprocessingSite } };

        // Expectations
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        _requestMapper.Setup(o => o.MapForCreate()).ReturnsAsync(request);

        _registrationService.Setup(o => o.CreateAsync(request)).ReturnsAsync(response);
        _reprocessorService.Setup(o => o.Registrations).Returns(_registrationService.Object);

        // Act
        var result = await _controller.ProvideSiteGridReference(model, actionButton) as RedirectResult;

        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Url.Should().Be(expectedReturnUrl);
    }

    [TestMethod]
    public async Task ManualAddressForServiceOfNotices_Get_ReturnsViewWithModel()
    {
        // Arrange
        var session = new ReprocessorRegistrationSession
        {
            Journey = ["address-for-notices", "enter-address-for-notices"],
            RegistrationApplicationSession = new()
            {
                ReprocessingSite = new ReprocessingSite
                {
                    TypeOfAddress = AddressOptions.DifferentAddress,
                    ServiceOfNotice = new ServiceOfNotice
                    {
                        SourcePage = "address-for-notices"
                    }
                }
            }
        };

        // Expectations
        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Act
        var result = await _controller.ManualAddressForServiceOfNotices();
        var viewResult = result as ViewResult;
        var backLink = _controller.ViewBag.BackLinkToDisplay as string;

        // Assert
        using (new AssertionScope())
        {
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Be("ManualAddressForServiceOfNotices");
            viewResult.Model.Should().BeOfType<ManualAddressForServiceOfNoticesViewModel>();
            backLink.Should().BeEquivalentTo("address-for-notices");
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
        var session = new ReprocessorRegistrationSession
        {
            Journey = ["address-for-notices", "enter-address-for-notices"],
            RegistrationApplicationSession = new()
            {
                ReprocessingSite = new ReprocessingSite
                {
                    TypeOfAddress = AddressOptions.DifferentAddress,
                    ServiceOfNotice = new ServiceOfNotice
                    {
                        SourcePage = "address-for-notices"
                    }
                }
            }
        };

        // Expectations
        // Expectations
        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);
        _validationService.Setup(v => v.ValidateAsync(model, CancellationToken.None))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.ManualAddressForServiceOfNotices(model, "SaveAndContinue");
        var viewResult = result as ViewResult;
        var backLink = _controller.ViewBag.BackLinkToDisplay as string;

        // Assert
        using (new AssertionScope())
        {
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().Be(model);
            backLink.Should().BeEquivalentTo("address-for-notices");
        }
    }

    [TestMethod]
    public async Task ManualAddressForServiceOfNotices_Post_SaveAndContinue_RedirectsCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var model = new ManualAddressForServiceOfNoticesViewModel
        {
            AddressLine1 = "address line 1",
            AddressLine2 = "address line 2",
            Postcode = "CV1 1TT",
            County = "West Midlands",
            TownOrCity = "Birmingham",
        };
        var session = new ReprocessorRegistrationSession
        {
            RegistrationId = id,
            Journey = ["address-for-notices", "enter-address-for-notices"],
            RegistrationApplicationSession = new()
            {
                ReprocessingSite = new ReprocessingSite
                {
                    TypeOfAddress = AddressOptions.DifferentAddress,
                    ServiceOfNotice = new ServiceOfNotice
                    {
                        SourcePage = "address-for-notices"
                    }
                }
            }
        };

        var expectedSession = new ReprocessorRegistrationSession
        {
            RegistrationId = id,
            Journey = ["address-for-notices", "enter-address-for-notices"],
            RegistrationApplicationSession = new()
            {
                ReprocessingSite = new ReprocessingSite
                {
                    TypeOfAddress = AddressOptions.DifferentAddress,
                    ServiceOfNotice = new ServiceOfNotice
                    {
                        Address = new(model.AddressLine1, model.AddressLine2, null, model.TownOrCity, model.County, null, model.Postcode),
                        TypeOfAddress = AddressOptions.DifferentAddress,
                        SourcePage = "address-for-notices"
                    }
                }
            }
        };

        // Expectations
        _validationService.Setup(v => v.ValidateAsync(model, CancellationToken.None))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        _registrationService.Setup(o => o.UpdateAsync(id, new UpdateRegistrationRequestDto
        {
            RegistrationId = id,
            ApplicationTypeId = ApplicationType.Reprocessor,
            BusinessAddress = new()
            {
                AddressLine1 = "address line 1"
            }
        })).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ManualAddressForServiceOfNotices(model, "SaveAndContinue");
        var redirectResult = result as RedirectResult;

        // Assert
        using (new AssertionScope())
        {
            redirectResult.Should().NotBeNull();
            redirectResult.Url.Should().Be("check-your-answers-for-contact-details");
            session.Should().BeEquivalentTo(expectedSession);
        }
    }

    [TestMethod]
    public async Task ManualAddressForServiceOfNotices_Post_SaveAndComeBackLater_RedirectsCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var model = new ManualAddressForServiceOfNoticesViewModel();
        var session = new ReprocessorRegistrationSession
        {
            RegistrationId = id,
            Journey = ["address-for-notices", "enter-address-for-notices"],
            RegistrationApplicationSession = new()
            {
                ReprocessingSite = new ReprocessingSite
                {
                    TypeOfAddress = AddressOptions.DifferentAddress,
                    ServiceOfNotice = new ServiceOfNotice
                    {
                        SourcePage = "address-for-notices"
                    }
                }
            }
        };

        var expectedSession = new ReprocessorRegistrationSession
        {
            RegistrationId = id,
            Journey = ["address-for-notices", "enter-address-for-notices"],
            RegistrationApplicationSession = new()
            {
                ReprocessingSite = new ReprocessingSite
                {
                    TypeOfAddress = AddressOptions.DifferentAddress,
                    ServiceOfNotice = new ServiceOfNotice
                    {
                        Address = new(model.AddressLine1, model.AddressLine2, null, model.TownOrCity, model.County, null, model.Postcode),
                        TypeOfAddress = AddressOptions.DifferentAddress,
                        SourcePage = "address-for-notices"
                    }
                }
            }
        };
        expectedSession.RegistrationApplicationSession.RegistrationTasks.SetTaskAsInProgress(TaskType.SiteAndContactDetails);

        // Expectations
        _validationService.Setup(v => v.ValidateAsync(model, CancellationToken.None))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        _registrationService.Setup(o => o.UpdateAsync(id, new UpdateRegistrationRequestDto
        {
            RegistrationId = id,
            ApplicationTypeId = ApplicationType.Reprocessor,
            BusinessAddress = new()
            {
                AddressLine1 = "address line 1"
            }
        })).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ManualAddressForServiceOfNotices(model, "SaveAndComeBackLater");
        var redirectResult = result as RedirectResult;
        var backLink = _controller.ViewBag.BackLinkToDisplay as string;

        // Assert
        using (new AssertionScope())
        {
            redirectResult.Should().NotBeNull();
            redirectResult.Url.Should().Be(PagePaths.ApplicationSaved);
            backLink.Should().BeEquivalentTo("address-for-notices");
            session.Should().BeEquivalentTo(expectedSession);
        }
    }

    [TestMethod]
    public async Task ProvideGridReferenceOfReprocessingSite_ShouldReturnView()
    {
        _session = new ReprocessorRegistrationSession();
        _session.RegistrationApplicationSession.ReprocessingSite.SetSiteGridReference("test");

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        // Act
        var result = await _controller.ProvideGridReferenceOfReprocessingSite();
        var model = (result as ViewResult).Model as ProvideGridReferenceOfReprocessingSiteViewModel;

        // Assert
        result.Should().BeOfType<ViewResult>();
        model.Should().BeOfType<ProvideGridReferenceOfReprocessingSiteViewModel>();
        model.GridReference.Should().Be("test");
    }

    [TestMethod]
    public async Task ProvideGridReferenceOfReprocessingSite_ShouldSetBackLink()
    {
        // Act
        var result = await _controller.ProvideGridReferenceOfReprocessingSite() as ViewResult;
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;

        // Assert
        result.Should().BeOfType<ViewResult>();

        backlink.Should().Be(PagePaths.AddressOfReprocessingSite);
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
    [DataRow("SaveAndContinue", PagePaths.AddressOfReprocessingSite)]
    [DataRow("SaveAndComeBackLater", PagePaths.AddressOfReprocessingSite)]
    public async Task ProvideGridReferenceOfReprocessingSite_OnSubmit_ShouldSetBackLink(string actionButton, string backLinkUrl)
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new CreateRegistrationDto
        {
            ApplicationTypeId = 1
        };

        var response = new CreateRegistrationResponseDto
        {
            Id = id
        };

        _session = new ReprocessorRegistrationSession() { Journey = new List<string> { PagePaths.CountryOfReprocessingSite, PagePaths.GridReferenceOfReprocessingSite } };

        // Expectations
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        _requestMapper.Setup(o => o.MapForCreate()).ReturnsAsync(request);

        _registrationService.Setup(o => o.CreateAsync(request)).ReturnsAsync(response);
        _reprocessorService.Setup(o => o.Registrations).Returns(_registrationService.Object);

        var model = new ProvideGridReferenceOfReprocessingSiteViewModel() { GridReference = "TS1245412545" };

        // Act
        await _controller.ProvideGridReferenceOfReprocessingSite(model, actionButton);
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;

        // Assert
        backlink.Should().Be(backLinkUrl);
    }

    [TestMethod]
    [DataRow("SaveAndContinue", PagePaths.AddressForNotices)]
    [DataRow("SaveAndComeBackLater", PagePaths.ApplicationSaved)]
    public async Task ProvideGridReferenceOfReprocessingSite_OnSubmit_ShouldRedirect(string actionButton, string expectedReturnUrl)
    {
        // Arrange
        var id = Guid.NewGuid();
        _session = new ReprocessorRegistrationSession() { Journey = new List<string> { "", PagePaths.GridReferenceOfReprocessingSite } };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        var request = new CreateRegistrationDto
        {
            ApplicationTypeId = 1
        };

        var response = new CreateRegistrationResponseDto
        {
            Id = id
        };

        _requestMapper.Setup(o => o.MapForCreate()).ReturnsAsync(request);

        _registrationService.Setup(o => o.CreateAsync(request)).ReturnsAsync(response);
        _reprocessorService.Setup(o => o.Registrations).Returns(_registrationService.Object);

        var model = new ProvideGridReferenceOfReprocessingSiteViewModel { GridReference = "1245412545" };

        // Act
        var result = await _controller.ProvideGridReferenceOfReprocessingSite(model, actionButton) as RedirectResult;

        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Url.Should().Be(expectedReturnUrl);
    }

    [TestMethod]
    public async Task ProvideGridReferenceOfReprocessingSite_ShouldSaveGridReferenceInSession()
    {
        // Arrange
        var gridReference = "TS1245412545";
        var model = new ProvideGridReferenceOfReprocessingSiteViewModel { GridReference = gridReference };
        var id = Guid.NewGuid();
        var request = new CreateRegistrationDto
        {
            ApplicationTypeId = 1
        };

        var response = new CreateRegistrationResponseDto
        {
            Id = id
        };

        _session = new ReprocessorRegistrationSession { Journey = new List<string> { PagePaths.CountryOfReprocessingSite, PagePaths.GridReferenceOfReprocessingSite } };

        // Expectations
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);
        _requestMapper.Setup(o => o.MapForCreate()).ReturnsAsync(request);

        _registrationService.Setup(o => o.CreateAsync(request)).ReturnsAsync(response);
        _reprocessorService.Setup(o => o.Registrations).Returns(_registrationService.Object);

        // Act
        await _controller.ProvideGridReferenceOfReprocessingSite(model, "SaveAndContinue");

        // Assert
        _session.RegistrationApplicationSession.ReprocessingSite!.SiteGridReference.Should().Be(gridReference);
    }

    [TestMethod]
    public async Task SelectAddressForServiceOfNotices_Get_ReturnsViewWithModel()
    {
        // Arrange
        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorRegistrationSession());

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
    public async Task ManualAddressForReprocessingSite_Get_NoAddressInSession_GoToAddressForReprocessingSite()
    {
        // Arrange
        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorRegistrationSession());

        // Act
        var result = await _controller.ManualAddressForReprocessingSite();
        var viewResult = result as RedirectResult;

        // Assert
        using (new AssertionScope())
        {
            viewResult.Should().NotBeNull();
            viewResult.Url.Should().BeEquivalentTo("address-of-reprocessing-site");
        }
    }

    [TestMethod]
    [DataRow("postcode-of-reprocessing-site")]
    [DataRow("grid-reference-for-entered-reprocessing-site")]
    public async Task ManualAddressForReprocessingSite_Get_Returns_Correct_Back_Navigation(string expectedBakcLink)
    {
        var session = new ReprocessorRegistrationSession();
        session.RegistrationApplicationSession.ReprocessingSite = new ReprocessingSite
        {
            TypeOfAddress = AddressOptions.DifferentAddress

        };
        session.RegistrationApplicationSession.ReprocessingSite.SourcePage = expectedBakcLink;

        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        // Act
        var result = await _controller.ManualAddressForReprocessingSite();
        var viewResult = result as ViewResult;

        var backLink = _controller.ViewBag.BackLinkToDisplay as string;
        // Assert
        backLink.Should().BeEquivalentTo(expectedBakcLink);
    }

    [TestMethod]
    public async Task ManualAddressForReprocessingSite_Get_TypeOfAddressIsDifferentAddress_ReturnViewAndModel()
    {
        // Arrange
        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorRegistrationSession
            {
                RegistrationApplicationSession = new()
                {
                    ReprocessingSite = new ReprocessingSite()
                    {
                        TypeOfAddress = AddressOptions.DifferentAddress
                    }
                }
            });

        // Act
        var result = await _controller.ManualAddressForReprocessingSite();
        var viewResult = result as ViewResult;

        // Assert
        using (new AssertionScope())
        {
            viewResult.Should().NotBeNull();
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
    public async Task ManualAddressForReprocessingSite_Post_EnsureValuesSaved_SaveAndContinue_RedirectsCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var model = new ManualAddressForReprocessingSiteViewModel
        {
            AddressLine1 = "address line 1",
            AddressLine2 = "address line 2",
            Postcode = "CV1 1TT",
            County = "West Midlands",
            TownOrCity = "Birmingham",
            SiteGridReference = "TF1234"
        };
        var session = new ReprocessorRegistrationSession
        {
            RegistrationApplicationSession = new()
            {
                ReprocessingSite = new ReprocessingSite
                {
                    Nation = UkNation.England,
                    TypeOfAddress = AddressOptions.DifferentAddress,
                    SourcePage = PagePaths.SelectAddressForReprocessingSite
                }
            }
        };

        _validationService.Setup(v => v.ValidateAsync(model, CancellationToken.None))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(session);

        var request = new CreateRegistrationDto
        {
            ApplicationTypeId = 1
        };

        var response = new CreateRegistrationResponseDto
        {
            Id = id
        };

        _requestMapper.Setup(o => o.MapForCreate()).ReturnsAsync(request);

        _registrationService.Setup(o => o.CreateAsync(request)).ReturnsAsync(response);
        _reprocessorService.Setup(o => o.Registrations).Returns(_registrationService.Object);

        var expectedSession = new ReprocessorRegistrationSession
        {
            RegistrationId = id,
            Journey = ["enter-reprocessing-site-address", "select-address-of-reprocessing-site"],
            RegistrationApplicationSession = new()
            {
                ReprocessingSite = new ReprocessingSite
                {
                    Nation = UkNation.England,
                    Address = new(model.AddressLine1, model.AddressLine2, null, model.TownOrCity, model.County, UkNation.England.GetDisplayName(), model.Postcode),
                    TypeOfAddress = AddressOptions.DifferentAddress,
                    SiteGridReference = "TF1234",
                    SourcePage = PagePaths.SelectAddressForReprocessingSite
                }
            }
        };

        // Act
        var result = await _controller.ManualAddressForReprocessingSite(model, "SaveAndContinue");
        var redirectResult = result as RedirectResult;

        // Assert
        using (new AssertionScope())
        {
            redirectResult.Should().NotBeNull();
            redirectResult.Url.Should().Be(PagePaths.AddressForNotices);
            session.Should().BeEquivalentTo(expectedSession);
        }
    }

    [TestMethod]
    public async Task ManualAddressForReprocessingSite_Post_SaveAndComeBackLater_RedirectsCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var model = new ManualAddressForReprocessingSiteViewModel();
        _validationService.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorRegistrationSession());

        var request = new CreateRegistrationDto
        {
            ApplicationTypeId = 1
        };

        var response = new CreateRegistrationResponseDto
        {
            Id = id
        };

        _requestMapper.Setup(o => o.MapForCreate()).ReturnsAsync(request);

        _registrationService.Setup(o => o.CreateAsync(request)).ReturnsAsync(response);
        _reprocessorService.Setup(o => o.Registrations).Returns(_registrationService.Object);


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
            .ReturnsAsync(new ReprocessorRegistrationSession());

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
        var result = await _controller.PostcodeForServiceOfNotices(model);
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
            .ReturnsAsync(new ReprocessorRegistrationSession());

        // Act
        var result = await _controller.PostcodeForServiceOfNotices(model);
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
            .ReturnsAsync(new ReprocessorRegistrationSession());

        // Act
        var result = await _controller.PostcodeForServiceOfNotices(model);
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
            .ReturnsAsync(new ReprocessorRegistrationSession());

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
        var result = await _controller.ConfirmNoticesAddress();
        var viewResult = result as ViewResult;
        // Assert
        using (new AssertionScope())
        {
            Assert.AreSame(typeof(ViewResult), result.GetType(), "Result should be of type ViewResult");
            viewResult!.Model.Should().BeOfType<ConfirmNoticesAddressViewModel>();
        }
    }

    [TestMethod]
    public async Task ConfirmNoticesAddress_Sets_BackLink_ReturnsExpectedViewResult()
    {
        // Act
        var result = await _controller.ConfirmNoticesAddress();
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;

        // Assert
        Assert.AreEqual(PagePaths.SelectAddressForServiceOfNotices, backlink);
    }

    [TestMethod]
    public async Task ConfirmNoticesAddress_OnSubmit_ReturnsExpectedViewResult()
    {
        var model = new ConfirmNoticesAddressViewModel();
        // Act
        var result = await _controller.ConfirmNoticesAddress(model);
        var viewResult = result as RedirectResult;
        // Assert
        using (new AssertionScope())
        {
            Assert.AreSame(typeof(RedirectResult), result.GetType(), "Result should be of type ViewResult");
        }
    }

    [TestMethod]
    public async Task ConfirmNoticesAddress_OnSubmit_Sets_BackLink_ReturnsExpectedViewResult()
    {
        var model = new ConfirmNoticesAddressViewModel();
        // Act
        var result = await _controller.ConfirmNoticesAddress(model);
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;

        // Assert
        Assert.AreEqual(PagePaths.SelectAddressForServiceOfNotices, backlink);
    }


    [TestMethod]
    public async Task CheckAnswers_Get_ReturnsViewWithModel()
    {
        // Arrange
        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorRegistrationSession());

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
        var model = new CheckAnswersViewModel
        {
            SiteGridReference = "AB1234567890",
            SiteLocation = UkNation.England,
            ReprocessingSiteAddress = new AddressViewModel
            {
                AddressLine1 = "Test Address Line 1",
                AddressLine2 = "Test Address Line 2",
                TownOrCity = "Test City",
                County = "Test County",
                Postcode = "G5 0US"
            },
            ServiceOfNoticesAddress = new AddressViewModel
            {
                AddressLine1 = "Test Address Line 1",
                AddressLine2 = "Test Address Line 2",
                TownOrCity = "Test City",
                County = "Test County",
                Postcode = "G5 0US"
            },
        };

        _sessionManagerMock.Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorRegistrationSession());

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
        // Arrange
        var session = new ReprocessorRegistrationSession
        {
            RegistrationApplicationSession = new()
            {
                WasteDetails = new()
                {
                    SelectedMaterials = [new() { Name = MaterialItem.Aluminium }]
                }
            }
        };

        // Expectations 
        _sessionManagerMock.Setup(o => o.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // Act
        var result = await _controller.SelectAuthorisationType(new Mock<IStringLocalizer<SelectAuthorisationType>>().Object);
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
        // Arrange
        var session = new ReprocessorRegistrationSession
        {
            RegistrationApplicationSession = new()
            {
                ReprocessingSite = new ReprocessingSite
                {
                     Nation = UkNation.England
                },
                WasteDetails = new()
                {
                    SelectedMaterials = [new() { Name = MaterialItem.Aluminium }],
                    SelectedAuthorisation = expectedResult,
                    
                }
            }
        };

        // Expectations 
        _sessionManagerMock.Setup(o => o.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        var materialPermitTypes = Enum.GetValues(typeof(MaterialPermitType))
                   .Cast<MaterialPermitType>()
                   .Select(e => new MaterialsPermitTypeDto
                   {
                       Id = (int)e,
                       Name = e.ToString()
                   })
                   .Where(x => x.Id > 0)
                   .ToList();

        _registrationMaterialService
            .Setup(x => x.GetMaterialsPermitTypesAsync())
            .ReturnsAsync(materialPermitTypes);

        // Act
        var result = await _controller.SelectAuthorisationType(new Mock<IStringLocalizer<SelectAuthorisationType>>().Object, nationCode);
        var viewResult = result as ViewResult;

        // Assert
        using (new AssertionScope())
        {
            Assert.AreSame(typeof(ViewResult), result.GetType(), "Result should be of type ViewResult");
           // (viewResult.Model as SelectAuthorisationTypeViewModel).AuthorisationTypes.Count.Should().Be(expectedResult);
        }
    }

    [TestMethod]
    public async Task SelectAuthorisationType_CurrentMaterialNull_ShouldRedirectToWastePermitExemptions()
    {
        // Arrange
        var session = new ReprocessorRegistrationSession();

        // Expectations 
        _sessionManagerMock.Setup(o => o.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // Act
        var result = await _controller.SelectAuthorisationType(new Mock<IStringLocalizer<SelectAuthorisationType>>().Object);

        var viewResult = result as RedirectResult;

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<RedirectResult>();
            viewResult!.Url.Should().BeEquivalentTo("select-materials-authorised-to-recycle");
        }
    }

    [TestMethod]
    public async Task SelectAuthorisationType_SetsBackLink_ReturnsExpectedViewResult()
    {
        // Arrange
        var session = new ReprocessorRegistrationSession
        {
            RegistrationApplicationSession = new()
            {
                WasteDetails = new()
                {
                    SelectedMaterials = [new() { Name = MaterialItem.Aluminium }]
                }
            }
        };

        // Expectations 
        _sessionManagerMock.Setup(o => o.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // Act
        var result = await _controller.SelectAuthorisationType(new Mock<IStringLocalizer<SelectAuthorisationType>>().Object);
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
        _session = new ReprocessorRegistrationSession() { Journey = new List<string> { PagePaths.CountryOfReprocessingSite, PagePaths.GridReferenceOfReprocessingSite } };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        var authorisationTypes = GetAuthorisationTypes();
        var index = authorisationTypes.IndexOf(authorisationTypes.FirstOrDefault(x => x.Id == 1)!);
        authorisationTypes[index].SelectedAuthorisationText = "testing";

        var model = new SelectAuthorisationTypeViewModel { SelectedAuthorisation = 1, AuthorisationTypes = authorisationTypes };

        // Act
        var result = _controller.SelectAuthorisationType(model, actionButton);
        var backlink = _controller.ViewBag.BackLinkToDisplay as string;
        // Assert

        backlink.Should().Be(backLinkUrl);
    }

    [TestMethod]
    [DataRow("SaveAndContinue", PagePaths.ExemptionReferences)]
    [DataRow("SaveAndComeBackLater", PagePaths.ApplicationSaved)]
    public async Task SelectAuthorisationType_OnSubmit_ShouldBeSuccessful(string actionButton, string expectedRedirectUrl)
    {
        //Arrange
        _session = new ReprocessorRegistrationSession() { Journey = new List<string> { PagePaths.CountryOfReprocessingSite, PagePaths.GridReferenceOfReprocessingSite } };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        var authorisationTypes = GetAuthorisationTypes();
        var index = authorisationTypes.IndexOf(authorisationTypes.FirstOrDefault(x => x.Id == 1)!);
        authorisationTypes[index].SelectedAuthorisationText = "testing";

        var model = new SelectAuthorisationTypeViewModel() { SelectedAuthorisation = 1, AuthorisationTypes = authorisationTypes };

        // Act
        var result = await _controller.SelectAuthorisationType(model, actionButton);
        var redirectResult = result as RedirectResult;

        // Assert
        using (new AssertionScope())
        {
            redirectResult.Should().NotBeNull();
            redirectResult.Url.Should().Be(expectedRedirectUrl);
        }
    }

    [TestMethod]
    [DataRow(5, "error_message_enter_permit_or_license_number")]
    [DataRow(2, "error_message_enter_permit_number")]
    [DataRow(3, "error_message_enter_permit_number")]
    [DataRow(4, "error_message_enter_permit_number")]
    public async Task SelectAuthorisationType_OnSubmit_ValidateModel_ShouldReturnModelError(int id, string expectedErrorMessage)
    {
        //Arrange
        _session = new ReprocessorRegistrationSession() { Journey = new List<string> { PagePaths.CountryOfReprocessingSite, PagePaths.GridReferenceOfReprocessingSite } };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        var authorisationTypes = GetAuthorisationTypes();
        var model = new SelectAuthorisationTypeViewModel() { SelectedAuthorisation = id, AuthorisationTypes = authorisationTypes };
        var index = authorisationTypes.IndexOf(authorisationTypes.FirstOrDefault(x => x.Id == id)!);

        // Act
        var result = _controller.SelectAuthorisationType(model, "SaveAndContinue");
        var modelState = _controller.ModelState;

        // Assert
        using (new AssertionScope())
        {
            Assert.AreEqual(1, modelState[$"AuthorisationTypes.SelectedAuthorisationText[{index}]"].Errors.Count);
            Assert.AreEqual(expectedErrorMessage, modelState[$"AuthorisationTypes.SelectedAuthorisationText[{index}]"].Errors[0].ErrorMessage);
        }
    }

    [TestMethod]
    public async Task ProvideWasteManagementLicense_ReturnsExpectedViewResult()
    {
        // Act
        var result = await _controller.ProvideWasteManagementLicense();
        var viewResult = result as ViewResult;

        // Assert
        using (new AssertionScope())
        {
            Assert.AreSame(typeof(ViewResult), result.GetType(), "Result should be of type ViewResult");
            viewResult.Model.Should().BeOfType<MaterialPermitViewModel>();
        }
    }

    [TestMethod]
    public async Task ProvideWasteManagementLicense_SetsBackLink_ReturnsExpectedViewResult()
    {
        // Act
        var result = await _controller.ProvideWasteManagementLicense();
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
        _session = new ReprocessorRegistrationSession() { Journey = new List<string> { PagePaths.PermitForRecycleWaste, PagePaths.WasteManagementLicense } };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session); ;

        var model = new MaterialPermitViewModel { SelectedFrequency = MaterialFrequencyOptions.PerYear, MaximumWeight = "10" };

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
        _session = new ReprocessorRegistrationSession() { Journey = new List<string> { PagePaths.PermitForRecycleWaste, PagePaths.WasteManagementLicense } };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);


        var model = new MaterialPermitViewModel { SelectedFrequency = MaterialFrequencyOptions.PerYear, MaximumWeight = "10" };

        // Act
        var result = await _controller.ProvideWasteManagementLicense(model, actionButton);
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
    [DataRow(null, "10", "Select if the authorised weight is per year, per month or per week", nameof(MaterialPermitViewModel.SelectedFrequency))]
    [DataRow(MaterialFrequencyOptions.PerYear, "0", "Weight must be a number greater than 0", nameof(MaterialPermitViewModel.MaximumWeight))]
    [DataRow(MaterialFrequencyOptions.PerYear, "11000000", "Weight must be a number less than 10,000,000", nameof(MaterialPermitViewModel.MaximumWeight))]
    [DataRow(MaterialFrequencyOptions.PerYear, "dsdsd", "Weight must be a number, like 100", nameof(MaterialPermitViewModel.MaximumWeight))]
    [DataRow(MaterialFrequencyOptions.PerYear, null, "Enter the maximum weight the permit authorises the site to accept and recycle", nameof(MaterialPermitViewModel.MaximumWeight))]
    public async Task ProvideWasteManagementLicense_OnSubmit_ValidateModel_ShouldReturnModelError(MaterialFrequencyOptions? selectedFrequency, string weight, string expectedErrorMessage, string modelStateKey, bool isCustomError = false)
    {
        //Arrange
        _session = new ReprocessorRegistrationSession() { Journey = new List<string> { PagePaths.PermitForRecycleWaste, PagePaths.WasteManagementLicense } };
        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(_session);

        var model = new MaterialPermitViewModel { SelectedFrequency = selectedFrequency, MaximumWeight = weight };

        ValidateViewModel(model);
        // Act
        var result = _controller.ProvideWasteManagementLicense(model, "SaveAndComeBackLater");
        var modelState = _controller.ModelState;
        // Assert

        // Assert
        using (new AssertionScope())
        {
            modelStateKey = isCustomError ? "" : modelStateKey;
            Assert.AreEqual(1, modelState[modelStateKey]!.Errors.Count);
            Assert.AreEqual(expectedErrorMessage, modelState[modelStateKey]!.Errors[0].ErrorMessage);
        }
    }


    private void ValidateViewModel(object model)
    {
        ValidationContext validationContext = new ValidationContext(model, null, null);
        List<ValidationResult> validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        foreach (ValidationResult validationResult in validationResults)
        {
            _controller.ControllerContext.ModelState.AddModelError(String.Join(", ", validationResult.MemberNames), validationResult.ErrorMessage!);
        }
    }

    private void SetupDefaultUserAndSessionMocks()
    {
        SetupMockSession();
        SetupMockHttpContext(CreateClaims(GetUserData()));
    }

    private void SetupMockSession()
    {
        _sessionManagerMock.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(new ReprocessorRegistrationSession());
    }

    private void SetupMockHttpContext(List<Claim> claims)
    {
        _userMock.Setup(x => x.Claims).Returns(claims);
        _httpContextMock.Setup(x => x.User).Returns(_userMock.Object);
        _controller.ControllerContext.HttpContext = _httpContextMock.Object;
    }

    private void SetupMockPostcodeLookup()
    {
        var addressList = new AddressList { Addresses = new List<App.DTOs.AddressLookup.Address>() };
        for (int i = 1; i < 3; i++)
        {
            var address = new App.DTOs.AddressLookup.Address
            {
                BuildingNumber = $"{i}",
                Street = "Test Street",
                County = "Test County",
                Locality = "Test Locality",
                Postcode = "T5 0ED",
                Town = "Test Town"
            };

            addressList.Addresses.Add(address);
        }

        _postcodeLookupService
            .Setup(x => x.GetAddressListByPostcodeAsync(It.IsAny<string>()))
            .ReturnsAsync(addressList);
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

    private static ReprocessorRegistrationSession CreateReprocessorRegistrationSession()
    {
        var userData = GetUserDateWithNationIdAndCompanyNumber();
        var registerApplicationSession = new RegistrationApplicationSession
        {
            ReprocessingSite = new ReprocessingSite
            {
                Address = new Address("Address line 1", "Address line 2", "Locality", "Town", "County", "Country", "CV12TT"),
                TypeOfAddress = AddressOptions.BusinessAddress
            }
        };
        var reprocessorRegistrationSession = new ReprocessorRegistrationSession
        {
            Journey = new List<string>(),
            UserData = userData,
            RegistrationApplicationSession = registerApplicationSession
        };

        return reprocessorRegistrationSession;
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

    private static List<AuthorisationTypes> GetAuthorisationTypes()
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