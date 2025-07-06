using Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Submission;
using Epr.Reprocessor.Exporter.UI.Helpers;
using Epr.Reprocessor.Exporter.UI.App.Enums.Accreditation;
using Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using System.Text;
using static Epr.Reprocessor.Exporter.UI.Controllers.AccreditationController;
using CheckAnswersViewModel = Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation.CheckAnswersViewModel;
using TaskStatus = Epr.Reprocessor.Exporter.UI.App.Enums.TaskStatus;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers
{
    [TestClass]
    public class AccreditationControllerTests
    {
        private UserData _userData;
        private AccreditationController _controller;
        private Mock<IStringLocalizer<SharedResources>> _mockLocalizer = new();
        private Mock<IAccountServiceApiClient> _mockAccountServiceClient = new();
        private Mock<IOptions<ExternalUrlOptions>> _mockExternalUrlOptions = new();
        private Mock<IOptions<GlobalVariables>> _mockGlobalVariables = new();
        private Mock<IAccreditationService> _mockAccreditationService = new();
        private Mock<ClaimsPrincipal> _claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        private Mock<IValidationService> _mockValidationService = new();
        private Mock<IUrlHelper> _mockUrlHelperMock = new();
        private Mock<IFileUploadService> _mockFileUploadService = new();
        private Mock<IFileDownloadService> _mockFileDownloadService = new();

        [TestInitialize]
        public void Setup()
        {
            _controller = new AccreditationController(
                _mockLocalizer.Object,
                _mockExternalUrlOptions.Object,
                _mockGlobalVariables.Object,
                _mockValidationService.Object,
                _mockAccountServiceClient.Object,
                _mockAccreditationService.Object,
                _mockFileUploadService.Object,
                _mockFileDownloadService.Object);

            _controller.Url = _mockUrlHelperMock.Object;
            _userData = GetUserData("Producer");
            SetupUserData(_userData);

            _mockGlobalVariables.Setup(g => g.Value).Returns(new GlobalVariables()
            {
                AccreditationFileUploadLimitInBytes = 20971520,
            });
        }

        private void SetupUserData(UserData userData)
        {
            var claims = new List<Claim>();
            if (userData != null)
            {
                claims.Add(new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(userData)));
            }
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAccredition"));
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
        }

        private UserData GetUserData(string organisationRole)
        {
            return new UserData
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "User",
                Organisations =
                [
                    new EPR.Common.Authorization.Models.Organisation
                    {
                        Name = "Some Organisation",
                        OrganisationNumber = "123456",
                        Id = Guid.NewGuid(),
                        OrganisationRole = organisationRole,
                        NationId = 1
                    }
                ]
            };
        }

        private static void SetupTempData(Controller controller, IDictionary<string, object>? initialData = null)
        {
            var httpContext = new DefaultHttpContext();
            var tempDataProvider = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            if (initialData != null)
            {
                foreach (var kvp in initialData)
                    tempDataProvider[kvp.Key] = kvp.Value;
            }
            controller.TempData = tempDataProvider;
        }

        #region ApplicationSaved
        [TestMethod]
        public async Task ApplicationSaved_ReturnsExpectedViewResult()
        {
            // Act
            var result = _controller.ApplicationSaved();

            // Assert
            Assert.AreSame(typeof(ViewResult), result.GetType(), "Result should be of type ViewResult");

        }
        #endregion

        #region EnsureAccreditation
        [TestMethod]
        public async Task EnsureAccreditation_Get_And_RedirectToTaskList()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var materialId = 2;
            var applicationTypeId = 1;

            _mockAccreditationService.Setup(x => x.GetOrCreateAccreditation(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(accreditationId);

            // Act
            var result = await _controller.EnsureAccreditation(materialId, applicationTypeId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.AccreditationTaskList);
            redirectResult.RouteValues.Count.Should().Be(1);
            redirectResult.RouteValues["AccreditationId"].Should().Be(accreditationId);
        }

        #endregion

        #region NotAnApprovedPerson

        [TestMethod]
        public async Task NotAnApprovedPerson_Get_ReturnsView()
        {
            // Arrange
            var usersApproved = new List<UserModel>
            {
                new UserModel { FirstName = "Joseph", LastName = "Bloggs", ServiceRoleId = 1 }
            };
            _mockAccountServiceClient.Setup(x => x.GetUsersForOrganisationAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(usersApproved);
            // Act
            var result = await _controller.NotAnApprovedPerson();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(NotAnApprovedPersonViewModel));
            var model = viewResult.ViewData.Model as NotAnApprovedPersonViewModel;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.ApprovedPersons.Count > 0);
        }

        #endregion

        #region CalendarYear

        [TestMethod]
        public async Task CalendarYear_Get_ReturnsView()
        {
            // Arrange
            _mockExternalUrlOptions.Setup(x => x.Value)
                .Returns(new ExternalUrlOptions { NationalPackagingWasteDatabase = "npwd" });

            // Act
            var result = _controller.CalendarYear();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(CalendarYearViewModel));
            var model = viewResult.ViewData.Model as CalendarYearViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual("npwd", model.NpwdLink);
        }

        #endregion

        #region PrnTonnage

        [TestMethod]
        public async Task PrnTonnage_Get_ReturnsView()
        {
            // Arrange
            _mockUrlHelperMock.Setup(u => u.RouteUrl(It.IsAny<UrlRouteContext>()))
                .Returns("backUrl");
            _mockAccreditationService.Setup(x => x.GetAccreditation(It.IsAny<Guid>()))
                .ReturnsAsync(new AccreditationDto
                {
                    MaterialName = "Steel",
                });

            // Act
            var result = await _controller.PrnTonnage(Guid.NewGuid());

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewData.Model.Should().BeOfType<PrnTonnageViewModel>();
            var model = viewResult.ViewData.Model as PrnTonnageViewModel;
            model.Should().NotBeNull();
            model.MaterialName.Should().Be("steel");
        }

        [TestMethod]
        public async Task PrnTonnage_Post_InvalidViewModel_ReturnsSameView()
        {
            // Arrange
            _controller.ModelState.AddModelError("PrnTonnage", "Required");
            var viewModel = new PrnTonnageViewModel { MaterialName = "steel", Action = "continue" };

            // Act
            var result = await _controller.PrnTonnage(viewModel);

            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewData.Model.Should().BeOfType<PrnTonnageViewModel>();
            var model = viewResult.ViewData.Model as PrnTonnageViewModel;
            model.Should().NotBeNull();
            model.Should().Be(viewModel);
        }

        [TestMethod]
        public async Task PrnTonnage_Post_ActionIsContinue_ReturnsRedirectToSelectAuthority()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            _mockAccreditationService.Setup(x => x.GetAccreditation(It.IsAny<Guid>()))
                .ReturnsAsync(new AccreditationDto
                {
                    MaterialName = "Steel",
                });
            var viewModel = new PrnTonnageViewModel { AccreditationId = accreditationId, MaterialName = "steel", Action = "continue" };

            var routeMetadata = new EndpointMetadataCollection(new RouteNameMetadata(AccreditationController.RouteIds.SelectPrnTonnage));
            var endPoint = new RouteEndpoint(
                requestDelegate: (ctx) => Task.CompletedTask,
                routePattern: RoutePatternFactory.Parse("/test"),
                order: 0,
                metadata: routeMetadata,
                displayName: null);

            _controller.HttpContext.SetEndpoint(endPoint);
            // Act
            var result = await _controller.PrnTonnage(viewModel);

            // Assert
            result.Should().BeOfType<RedirectToRouteResult>();
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.SelectAuthorityPRNs);
            redirectResult.RouteValues.Count.Should().Be(1);
            redirectResult.RouteValues["AccreditationId"].Should().Be(accreditationId);
        }

        [TestMethod]
        public async Task PrnTonnage_Post_ActionIsSave_ReturnsRedirectToApplicationSaved()
        {
            // Arrange
            _mockAccreditationService.Setup(x => x.GetAccreditation(It.IsAny<Guid>()))
                .ReturnsAsync(new AccreditationDto
                {
                    MaterialName = "Steel",
                });
            var viewModel = new PrnTonnageViewModel { MaterialName = "steel", Action = "save" };

            // Act
            var result = await _controller.PrnTonnage(viewModel);

            // Assert
            result.Should().BeOfType<RedirectToRouteResult>();
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.ApplicationSaved);
        }

        [TestMethod]
        public async Task PrnTonnage_Post_ActionIsUnknown_ReturnsBadRequest()
        {
            // Arrange
            _mockAccreditationService.Setup(x => x.GetAccreditation(It.IsAny<Guid>()))
                .ReturnsAsync(new AccreditationDto
                {
                    MaterialName = "Steel",
                });
            var viewModel = new PrnTonnageViewModel { MaterialName = "steel", Action = "unknown" };

            // Act
            var result = await _controller.PrnTonnage(viewModel);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Invalid action supplied.");
        }
        #endregion

        #region SelectAuthority

        [TestMethod]
        public async Task SelectAuthority_Get_ReturnsViewWithModel()
        {
            // Act
            var accreditationId = Guid.NewGuid();
            _mockAccreditationService.Setup(x => x.GetOrganisationUsers(It.IsAny<UserData>(), It.IsAny<bool>()))
                .ReturnsAsync(new List<ManageUserDto> { new ManageUserDto
                {
                    PersonId = Guid.NewGuid(),
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test@user.com"
                } });

            _mockAccreditationService.Setup(x => x.GetAccreditation(It.IsAny<Guid>()))
                .ReturnsAsync(new AccreditationDto
                {
                    ExternalId = accreditationId,
                    ApplicationTypeId = (int)ApplicationType.Reprocessor,
                    MaterialName = "Steel",

                    AccreditationStatusId = 1,
                    AccreditationYear = 2024,
                    OrganisationId = Guid.NewGuid(),
                    RegistrationMaterialId = 5
                });

            var homeUrl = "/epr-prn/";
            _mockUrlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                .Returns(homeUrl);

            var result = await _controller.SelectAuthority(accreditationId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as SelectAuthorityViewModel;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Authorities.Count > 0);
            Assert.AreEqual("Steel", model.Accreditation.MaterialName);
            Assert.AreEqual("23 Ruby Street", model.SiteAddress);
        }

        [TestMethod]
        public async Task SelectAuthority_Post_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            _controller.ModelState.AddModelError("SelectedAuthorities", "Required");

            var model = new SelectAuthorityViewModel()
            {
                Accreditation = new AccreditationDto
                {
                    ExternalId = accreditationId,
                    ApplicationTypeId = (int)ApplicationType.Reprocessor,
                    MaterialName = "Steel"
                },
                Action = "continue"
            };

            _mockValidationService.Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(new List<ValidationFailure>
                {
                    new ValidationFailure("SelectedAuthorities", "Required")
                }));

            // Act
            var result = await _controller.SelectAuthority(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            // Assert.AreEqual(model, result.Model);
        }

        [TestMethod]
        public async Task SelectAuthority_Post_ValidModelState_ContinueAction_RedirectsToCheckAnswers()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var model = new SelectAuthorityViewModel()
            {
                Accreditation = new AccreditationDto
                {
                    ExternalId = accreditationId,
                    ApplicationTypeId = (int)ApplicationType.Reprocessor,
                    MaterialName = "Steel"
                },
                Action = "continue"
            };

            _mockValidationService.Setup(v => v.ValidateAsync(model, default))
              .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            // Act
            var result = await _controller.SelectAuthority(model);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<RedirectToRouteResult>();
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.CheckAnswersPRNs);
            redirectResult.RouteValues.Count.Should().Be(1);
            redirectResult.RouteValues["AccreditationId"].Should().Be(accreditationId);
        }

        [TestMethod]
        public async Task SelectAuthority_Post_ValidModelState_SaveAction_RedirectsToApplicationSaved()
        {
            // Arrange
            var model = new SelectAuthorityViewModel()
            {
                Accreditation = new AccreditationDto
                {
                    ExternalId = new Guid(),
                    ApplicationTypeId = (int)ApplicationType.Reprocessor,
                    MaterialName = "Steel"
                },
                Action = "save"
            };

            _mockValidationService.Setup(v => v.ValidateAsync(model, default))
             .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            // Act
            var result = await _controller.SelectAuthority(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.ApplicationSaved);
            redirectResult.RouteValues.Should().BeNull();
        }

        [TestMethod]
        public async Task SelectAuthority_Post_InvalidSelectedAuthoritiesCount_ReturnsView()
        {
            // Arrange
            var model = new SelectAuthorityViewModel
            {
                Action = "continue",
                SelectedAuthorities = new List<string>(), // No authorities selected
                ApplicationType = ApplicationType.Reprocessor,
                Accreditation = new AccreditationDto
                {
                    ExternalId = new Guid(),
                    ApplicationTypeId = (int)ApplicationType.Reprocessor,
                    MaterialName = "Steel"
                },
            };

            _mockValidationService.Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(new List<ValidationFailure>
                {
                    new ValidationFailure("SelectedAuthorities", "Required")
                }));

            // Simulate model validation
            var validationContext = new ValidationContext(model);
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            if (!Validator.TryValidateObject(model, validationContext, validationResults, true))
            {
                foreach (var validationResult in validationResults)
                {
                    _controller.ModelState.AddModelError(string.Empty, validationResult.ErrorMessage);
                }
            }

            // Act
            var result = await _controller.SelectAuthority(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Expected a ViewResult to be returned.");
            Assert.AreEqual(model, result.Model, "Expected the same model to be returned.");
            Assert.AreEqual(0, (result.Model as SelectAuthorityViewModel).SelectedAuthoritiesCount);
            Assert.IsFalse(_controller.ModelState.IsValid, "Expected ModelState to be invalid.");
        }
        #endregion

        #region CheckAnswers
        [TestMethod]
        public async Task CheckAnswers_Get_ReturnsViewResult()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var personId = Guid.NewGuid();
            _mockAccreditationService.Setup(x => x.GetAccreditation(It.IsAny<Guid>()))
                .ReturnsAsync(new AccreditationDto
                {
                    PrnTonnage = 500
                });

            _mockAccreditationService.Setup(x => x.GetAccreditationPrnIssueAuths(It.IsAny<Guid>()))
                .ReturnsAsync(
                    [
                        new AccreditationPrnIssueAuthDto
                        {
                            PersonExternalId = personId,
                            AccreditationExternalId = accreditationId
                        }
                    ]
                );

            _mockAccreditationService.Setup(x => x.GetOrganisationUsers(It.IsAny<UserData>(), It.IsAny<bool>()))
                .ReturnsAsync(
                    [
                        new ManageUserDto
                        {
                            PersonId = personId,
                            FirstName = "First",
                            LastName = "Last"
                        }
                    ]
                );

            var backUrl = $"/epr-prn/accreditation/authority-to-issue-prns/{accreditationId}";
            _mockUrlHelperMock.Setup(u => u.RouteUrl(It.IsAny<UrlRouteContext>()))
                .Returns(backUrl);

            // Act
            var result = await _controller.CheckAnswers(accreditationId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(CheckAnswersViewModel));
            var model = viewResult.ViewData.Model as CheckAnswersViewModel;
            Assert.IsNotNull(model);

            model.AccreditationId.Should().Be(accreditationId);
            model.PrnTonnage.Should().Be(500);
            model.AuthorisedUsers.Should().Be("First Last");

            var backlink = _controller.ViewBag.BackLinkToDisplay as string;
            backlink.Should().Be(backUrl);
        }

        [TestMethod]
        public async Task CheckAnswers_Get_Returns_Empty_ViewResult()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var personId = Guid.NewGuid();

            AccreditationDto accreditationDto = null!;
            _mockAccreditationService.Setup(x => x.GetAccreditation(It.IsAny<Guid>()))
                .ReturnsAsync(accreditationDto);

            List<AccreditationPrnIssueAuthDto> accreditationPrnIssueAuthDtos = null!;
            _mockAccreditationService.Setup(x => x.GetAccreditationPrnIssueAuths(It.IsAny<Guid>()))
                .ReturnsAsync(accreditationPrnIssueAuthDtos);

            List<ManageUserDto> manageUserDtos = null!;
            _mockAccreditationService.Setup(x => x.GetOrganisationUsers(It.IsAny<UserData>(), It.IsAny<bool>()))
                .ReturnsAsync(manageUserDtos);

            var backUrl = $"/epr-prn/accreditation/authority-to-issue-prns/{accreditationId}";
            _mockUrlHelperMock.Setup(u => u.RouteUrl(It.IsAny<UrlRouteContext>()))
                .Returns(backUrl);

            var routeMetadata = new EndpointMetadataCollection(new RouteNameMetadata(AccreditationController.RouteIds.CheckAnswersPRNs));
            var endPoint = new RouteEndpoint(
                requestDelegate: (ctx) => Task.CompletedTask,
                routePattern: RoutePatternFactory.Parse("/test"),
                order: 0,
                metadata: routeMetadata,
                displayName: null);

            _controller.HttpContext.SetEndpoint(endPoint);


            // Act
            var result = await _controller.CheckAnswers(accreditationId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(CheckAnswersViewModel));
            var model = viewResult.ViewData.Model as CheckAnswersViewModel;
            Assert.IsNotNull(model);

            model.AccreditationId.Should().Be(accreditationId);
            model.PrnTonnage.Should().Be(null);
            model.AuthorisedUsers.Should().Be(string.Empty);
            model.TonnageChangeRoutePath.Should().Be(AccreditationController.RouteIds.SelectPrnTonnage);
            model.AuthorisedUserChangeRoutePath.Should().Be(AccreditationController.RouteIds.SelectAuthorityPRNs);
            model.FormPostRouteName.Should().Be(AccreditationController.RouteIds.CheckAnswersPRNs);

            var backlink = _controller.ViewBag.BackLinkToDisplay as string;
            backlink.Should().Be(backUrl);
        }

        [TestMethod]
        public async Task CheckAnswers_Post_ActionIsContinue_ReturnsRedirectToTaskList()
        {
            // Arrange
            var viewModel = new CheckAnswersViewModel { Subject = "PRN", AccreditationId = Guid.NewGuid(), PrnTonnage = 500, AuthorisedUsers = "First Last, Test User", Action = "continue" };



            _mockAccreditationService.Setup(x => x.GetAccreditation(It.IsAny<Guid>()))
                .ReturnsAsync(new AccreditationDto
                {
                    PrnTonnage = 500
                });

            // Act
            var result = await _controller.CheckAnswers(viewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var redirectResult = result as RedirectToRouteResult;
            Assert.IsNotNull(redirectResult);
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.AccreditationTaskList);
            redirectResult.RouteValues.Count.Should().Be(1);
            redirectResult.RouteValues["AccreditationId"].Should().Be(viewModel.AccreditationId);
        }

        [TestMethod]
        public async Task CheckAnswers_Post_ActionIsSave_ReturnsRedirectToApplicationSaved()
        {
            // Arrange
            var viewModel = new CheckAnswersViewModel { AccreditationId = Guid.NewGuid(), PrnTonnage = 500, AuthorisedUsers = "First Last, Test User", Action = "save" };

            // Act
            var result = await _controller.CheckAnswers(viewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var redirectResult = result as RedirectToRouteResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(AccreditationController.RouteIds.ApplicationSaved, redirectResult.RouteName);
        }

        [TestMethod]
        public async Task CheckAnswers_Post_ActionIsUnknown_ReturnsBadRequest()
        {
            // Arrange
            var viewModel = new CheckAnswersViewModel { AccreditationId = Guid.NewGuid(), PrnTonnage = 500, AuthorisedUsers = "First Last, Test User" };

            // Act
            var result = await _controller.CheckAnswers(viewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual("Invalid action supplied.", (result as BadRequestObjectResult).Value);
        }

        [TestMethod]
        public async Task CheckAnswers_Post_ActionIsContinue_WithPERNSubject_RedirectsToExporterAccreditationTaskList()
        {
            // Arrange
            var viewModel = new CheckAnswersViewModel
            {
                AccreditationId = Guid.NewGuid(),
                PrnTonnage = 100,
                AuthorisedUsers = "Test User",
                Action = "continue",
                Subject = "PERN"
            };

            _mockAccreditationService.Setup(x => x.GetAccreditation(It.IsAny<Guid>()))
                .ReturnsAsync(new AccreditationDto
                {
                    PrnTonnage = 500
                });

            // Act
            var result = await _controller.CheckAnswers(viewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var redirectResult = result as RedirectToRouteResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(AccreditationController.RouteIds.ExporterAccreditationTaskList, redirectResult.RouteName);
        }
        #endregion

        #region BusinessPlan
        [TestMethod]
        public async Task BusinessPlan_Get_ReturnsViewResult_WithBusinessPlanViewModel()
        {
            var testId = Guid.NewGuid();
            _mockAccreditationService
                .Setup(s => s.GetAccreditation(testId))
                .ReturnsAsync(new AccreditationDto
                {
                    ExternalId = testId,
                    MaterialName = "Plastic"
                });

            var result = await _controller.BusinessPlan(testId);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(BusinessPlanViewModel));
        }

        [TestMethod]
        public async Task BusinessPlan_Post_InvalidModelState_ReturnsView()
        {
            var model = new BusinessPlanViewModel { ExternalId = Guid.NewGuid() };
            _controller.ModelState.AddModelError("TestError", "Some error");

            var result = await _controller.BusinessPlan(model);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreSame(model, viewResult.Model);
        }

        [TestMethod]
        public async Task BusinessPlan_Post_ValidModel_ContinueAction_Redirects()
        {
            var model = new BusinessPlanViewModel
            {
                ExternalId = Guid.NewGuid(),
                InfrastructurePercentage = "20",
                PackagingWastePercentage = "20",
                BusinessCollectionsPercentage = "20",
                CommunicationsPercentage = "10",
                NewMarketsPercentage = "15",
                NewUsesPercentage = "10",
                OtherPercentage = "5",
                Subject = "PRN",
                Action = "continue"
            };

            _mockAccreditationService
                .Setup(s => s.GetAccreditation(model.ExternalId))
                .ReturnsAsync(new AccreditationDto
                {
                    ExternalId = model.ExternalId,
                    AccreditationStatusId = 1,
                    AccreditationYear = 2024,
                    OrganisationId = Guid.NewGuid(),
                    RegistrationMaterialId = 5
                });

            _mockAccreditationService
                .Setup(s => s.UpsertAccreditation(It.IsAny<AccreditationRequestDto>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.BusinessPlan(model);

            var redirectResult = result as RedirectToRouteResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(RouteIds.MoreDetailOnBusinessPlanPRNs, redirectResult.RouteName);
        }

        [TestMethod]
        public async Task BusinessPlan_Post_ValidModel_SaveAction_Redirects()
        {
            var model = new BusinessPlanViewModel
            {
                ExternalId = Guid.NewGuid(),
                InfrastructurePercentage = "30",
                PackagingWastePercentage = "20",
                BusinessCollectionsPercentage = "10",
                CommunicationsPercentage = "10",
                NewMarketsPercentage = "10",
                NewUsesPercentage = "10",
                OtherPercentage = "10",
                Action = "save"
            };

            _mockAccreditationService
                .Setup(s => s.GetAccreditation(model.ExternalId))
                .ReturnsAsync(new AccreditationDto());

            _mockAccreditationService
                .Setup(s => s.UpsertAccreditation(It.IsAny<AccreditationRequestDto>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.BusinessPlan(model);

            var redirectResult = result as RedirectToRouteResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(RouteIds.ApplicationSaved, redirectResult.RouteName);
        }

        [TestMethod]
        public async Task BusinessPlan_Post_InvalidAction_ReturnsBadRequest()
        {
            var model = new BusinessPlanViewModel
            {
                ExternalId = Guid.NewGuid(),
                InfrastructurePercentage = "50",
                PackagingWastePercentage = "50",
                Action = "unknown"
            };

            _mockAccreditationService
                .Setup(s => s.GetAccreditation(model.ExternalId))
                .ReturnsAsync(new AccreditationDto());

            _mockAccreditationService
                .Setup(s => s.UpsertAccreditation(It.IsAny<AccreditationRequestDto>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.BusinessPlan(model);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task BusinessPlan_Post_SetsBusinessPlanConfirmedFlagToFalse()
        {
            // Arrange
            var model = new BusinessPlanViewModel
            {
                ExternalId = Guid.NewGuid(),
                InfrastructurePercentage = "30",
                PackagingWastePercentage = "20",
                BusinessCollectionsPercentage = "10",
                CommunicationsPercentage = "10",
                NewMarketsPercentage = "10",
                NewUsesPercentage = "10",
                OtherPercentage = "10",
                Action = "save"
            };

            _mockAccreditationService
                .Setup(s => s.GetAccreditation(model.ExternalId))
                .ReturnsAsync(new AccreditationDto());

            // Act
            var result = await _controller.BusinessPlan(model);

            // Assert
            _mockAccreditationService.Verify(x => x.UpsertAccreditation(It.Is<AccreditationRequestDto>(x => x.BusinessPlanConfirmed == false)), Times.Once);
        }

        #endregion

        #region MoreDetailOnBusinessPlan

        [TestMethod]
        public async Task MoreDetailOnBusinessPlan_Get_WithValidId_ReturnsPopulatedViewModel()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var accreditation = new AccreditationDto
            {
                ExternalId = accreditationId,
                InfrastructurePercentage = 10,
                InfrastructureNotes = "Infra note",
                PackagingWastePercentage = 0,
                BusinessCollectionsPercentage = 5,
                BusinessCollectionsNotes = "Biz note",
                CommunicationsPercentage = 0,
                NewMarketsPercentage = 20,
                NewMarketsNotes = "Market note",
                NewUsesPercentage = 0
            };

            _mockAccreditationService
                .Setup(x => x.GetAccreditation(accreditationId))
                .ReturnsAsync(accreditation);

            // Act
            var result = await _controller.MoreDetailOnBusinessPlan(accreditationId);

            // Assert
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();

            var model = viewResult.Model as MoreDetailOnBusinessPlanViewModel;
            model.Should().NotBeNull();

            model.AccreditationId.Should().Be(accreditation.ExternalId);
            model.Subject.Should().Be("PERN");
            model.ShowInfrastructure.Should().BeTrue();
            model.Infrastructure.Should().Be("Infra note");
            model.ShowBusinessCollections.Should().BeTrue();
            model.BusinessCollections.Should().Be("Biz note");
            model.ShowNewMarkets.Should().BeTrue();
            model.NewMarkets.Should().Be("Market note");
            model.ShowCommunications.Should().BeFalse();
        }

        [TestMethod]
        public async Task MoreDetailOnBusinessPlan_Get_ReprocessorRouteButExporterAccreditation_ThrowsInvalidOperationException()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var accreditation = new AccreditationDto
            {
                ApplicationTypeId = (int)ApplicationType.Reprocessor
            };

            _mockAccreditationService
                .Setup(x => x.GetAccreditation(accreditationId))
                .ReturnsAsync(accreditation);

            // Set controller route to be the Exporter route.
            var routeMetadata = new EndpointMetadataCollection(new RouteNameMetadata(AccreditationController.RouteIds.MoreDetailOnBusinessPlanPERNs));
            var endPoint = new RouteEndpoint(
                requestDelegate: (ctx) => Task.CompletedTask,
                routePattern: RoutePatternFactory.Parse("/test"),
                order: 0,
                metadata: routeMetadata,
                displayName: null);

            _controller.HttpContext.SetEndpoint(endPoint);

            // Act
            Func<Task> act = async () => await _controller.MoreDetailOnBusinessPlan(accreditationId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [TestMethod]
        public async Task MoreDetailOnBusinessPlan_Get_ExporterRouteButReprocessorAccreditation_ThrowsInvalidOperationException()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var accreditation = new AccreditationDto
            {
                ApplicationTypeId = (int)ApplicationType.Exporter
            };

            _mockAccreditationService
                .Setup(x => x.GetAccreditation(accreditationId))
                .ReturnsAsync(accreditation);

            // Set controller route to be the Reprocessor route.
            var routeMetadata = new EndpointMetadataCollection(new RouteNameMetadata(AccreditationController.RouteIds.MoreDetailOnBusinessPlanPRNs));
            var endPoint = new RouteEndpoint(
                requestDelegate: (ctx) => Task.CompletedTask,
                routePattern: RoutePatternFactory.Parse("/test"),
                order: 0,
                metadata: routeMetadata,
                displayName: null);

            _controller.HttpContext.SetEndpoint(endPoint);

            // Act
            Func<Task> act = async () => await _controller.MoreDetailOnBusinessPlan(accreditationId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [TestMethod]
        public async Task MoreDetailOnBusinessPlan_Post_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var viewModel = new MoreDetailOnBusinessPlanViewModel
            {
                AccreditationId = Guid.NewGuid(),
                ApplicationTypeId = (int)ApplicationType.Reprocessor,
                Subject = "PRN",
                Action = "continue"
            };

            _controller.ModelState.AddModelError("Infrastructure", "Too long");

            // Act
            var result = await _controller.MoreDetailOnBusinessPlan(viewModel);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as MoreDetailOnBusinessPlanViewModel;
            model.Should().NotBeNull();
            model.Should().BeSameAs(viewModel);
            model.Subject.Should().Be("PRN");
        }

        [TestMethod]
        public async Task MoreDetailOnBusinessPlan_Post_TypeIsReprocessorAndActionIsContinue_RedirectsToCheckBusinessPlanPRN()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var viewModel = new MoreDetailOnBusinessPlanViewModel
            {
                AccreditationId = accreditationId,
                Action = "continue"
            };

            _mockAccreditationService
                .Setup(x => x.GetAccreditation(accreditationId))
                .ReturnsAsync(new AccreditationDto
                {
                    ApplicationTypeId = (int)ApplicationType.Reprocessor,
                });

            // Act
            var result = await _controller.MoreDetailOnBusinessPlan(viewModel);

            // Assert
            _mockAccreditationService.Verify(x => x.UpsertAccreditation(It.IsAny<AccreditationRequestDto>()), Times.Once);
            var redirect = result as RedirectToRouteResult;
            redirect.Should().NotBeNull();
            redirect.RouteName.Should().Be(AccreditationController.RouteIds.CheckBusinessPlanPRN);
        }

        [TestMethod]
        public async Task MoreDetailOnBusinessPlan_Post_TypeIsExporterAndActionIsContinue_RedirectsToCheckBusinessPlanPERN()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var viewModel = new MoreDetailOnBusinessPlanViewModel
            {
                AccreditationId = accreditationId,
                Action = "continue"
            };

            _mockAccreditationService
                .Setup(x => x.GetAccreditation(accreditationId))
                .ReturnsAsync(new AccreditationDto
                {
                    ApplicationTypeId = (int)ApplicationType.Exporter,
                });

            // Act
            var result = await _controller.MoreDetailOnBusinessPlan(viewModel);

            // Assert
            _mockAccreditationService.Verify(x => x.UpsertAccreditation(It.IsAny<AccreditationRequestDto>()), Times.Once);
            var redirect = result as RedirectToRouteResult;
            redirect.Should().NotBeNull();
            redirect.RouteName.Should().Be(AccreditationController.RouteIds.CheckBusinessPlanPERN);
        }

        [TestMethod]
        public async Task MoreDetailOnBusinessPlan_Post_ActionIsSave_RedirectsToApplicationSaved()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var viewModel = new MoreDetailOnBusinessPlanViewModel
            {
                AccreditationId = accreditationId,
                Action = "save"
            };

            _mockAccreditationService
                .Setup(x => x.GetAccreditation(accreditationId))
                .ReturnsAsync(new AccreditationDto());

            // Act
            var result = await _controller.MoreDetailOnBusinessPlan(viewModel);

            // Assert
            _mockAccreditationService.Verify(x => x.UpsertAccreditation(It.IsAny<AccreditationRequestDto>()), Times.Once);
            var redirect = result as RedirectToRouteResult;
            redirect.Should().NotBeNull();
            redirect.RouteName.Should().Be(AccreditationController.RouteIds.ApplicationSaved);
        }

        [TestMethod]
        public async Task MoreDetailOnBusinessPlan_Post_InvalidAction_ReturnsBadRequest()
        {
            // Arrange
            var viewModel = new MoreDetailOnBusinessPlanViewModel
            {
                AccreditationId = Guid.NewGuid(),
                Action = "invalid"
            };

            _mockAccreditationService
                .Setup(x => x.GetAccreditation(viewModel.AccreditationId))
                .ReturnsAsync(new AccreditationDto());

            // Act
            var result = await _controller.MoreDetailOnBusinessPlan(viewModel);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest.Value.Should().Be("Invalid action supplied.");
        }

        [TestMethod]
        public async Task MoreDetailOnBusinessPlan_Post_SetsNotesToNull_WhenPercentageIsZero()
        {
            // Arrange
            var viewModel = new MoreDetailOnBusinessPlanViewModel
            {
                AccreditationId = Guid.NewGuid(),
                Action = "continue",
                Infrastructure = "Some notes",
                PriceSupport = "Some notes",
                BusinessCollections = "Some notes",
                Communications = "Some notes",
                NewMarkets = "Some notes",
                NewUses = "Some notes",
                Other = "Some notes"
            };

            _mockAccreditationService
                .Setup(x => x.GetAccreditation(viewModel.AccreditationId))
                .ReturnsAsync(new AccreditationDto()
                {
                    ApplicationTypeId = (int)ApplicationType.Reprocessor,
                    InfrastructurePercentage = 0,
                    PackagingWastePercentage = 0,
                    BusinessCollectionsPercentage = 0,
                    CommunicationsPercentage = 0,
                    NewMarketsPercentage = 0,
                    NewUsesPercentage = 0,
                    OtherPercentage = 0,
                });

            // Act
            var result = await _controller.MoreDetailOnBusinessPlan(viewModel);

            // Assert
            _mockAccreditationService.Verify(x => x.UpsertAccreditation(It.Is<AccreditationRequestDto>(x =>
                x.InfrastructureNotes == null &&
                x.PackagingWasteNotes == null &&
                x.BusinessCollectionsNotes == null &&
                x.CommunicationsNotes == null &&
                x.NewMarketsNotes == null &&
                x.NewUsesNotes == null &&
                x.OtherNotes == null)), Times.Once);
        }

        [TestMethod]
        public async Task MoreDetailOnBusinessPlan_Post_SetsBusinessPlanConfirmedFlagToFalse()
        {
            // Arrange
            var viewModel = new MoreDetailOnBusinessPlanViewModel
            {
                AccreditationId = Guid.NewGuid(),
                Action = "continue",
                Infrastructure = "Some notes",
                PriceSupport = "Some notes",
                BusinessCollections = "Some notes",
                Communications = "Some notes",
                NewMarkets = "Some notes",
                NewUses = "Some notes",
                Other = "Some notes"
            };

            _mockAccreditationService
                .Setup(s => s.GetAccreditation(viewModel.AccreditationId))
                .ReturnsAsync(new AccreditationDto());

            // Act
            var result = await _controller.MoreDetailOnBusinessPlan(viewModel);

            // Assert
            _mockAccreditationService.Verify(x => x.UpsertAccreditation(It.Is<AccreditationRequestDto>(x => x.BusinessPlanConfirmed == false)), Times.Once);
        }

        #endregion

        #region ApplyForAccreditation


        [TestMethod]
        public void ApplyForAccreditation_ReturnsViewResult()
        {
            // Act
            var result = _controller.ApplyforAccreditation();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult), "Expected a ViewResult to be returned.");
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult, "Expected the ViewResult to not be null.");
        }

        [TestMethod]
        public void ApplyForAccreditation_ViewModelIsNull_ReturnsView()
        {
            // Act
            var result = _controller.ApplyforAccreditation() as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Expected a ViewResult to be returned.");
            Assert.IsNotNull(result.Model, "Expected the ViewModel to be returned.");
        }
        #endregion

        #region TaskList
        [TestMethod]
        public async Task WhenBasicUser_TaskList_ReturnsViewResult_WithApprovedPersonList()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var personId = Guid.NewGuid();
            _userData.ServiceRoleId = (int)ServiceRole.Basic;
            var usersApproved = new List<UserModel>
            {
                new() { FirstName = "Joseph", LastName = "Bloggs" }
            };
            _mockAccountServiceClient.Setup(x => x.GetUsersForOrganisationAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(usersApproved);

            _mockAccreditationService.Setup(x => x.GetAccreditation(It.IsAny<Guid>()))
                .ReturnsAsync(new AccreditationDto
                {
                    ExternalId = accreditationId,
                    PrnTonnage = 500,
                    ApplicationTypeId = (int)ApplicationType.Reprocessor
                });

            _mockAccreditationService.Setup(x => x.GetAccreditationPrnIssueAuths(It.IsAny<Guid>()))
                .ReturnsAsync(
                    [
                        new AccreditationPrnIssueAuthDto
                        {
                            PersonExternalId = personId,
                            AccreditationExternalId = accreditationId
                        }
                    ]
                );

            // Act
            var result = await _controller.TaskList(accreditationId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            var model = viewResult.ViewData.Model as TaskListViewModel;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.PeopleCanSubmitApplication.ApprovedPersons.Any());
        }

        [TestMethod]
        [DataRow((int)ServiceRole.Approved, DisplayName = "Approved user")]
        [DataRow((int)ServiceRole.Delegated, DisplayName = "Delegated user")]
        public async Task WhenAuthorisedUser_TaskList_ReturnsViewResult_WithoutApprovedPersonList(int serviceRoleId)
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var personId = Guid.NewGuid();
            _userData.ServiceRoleId = serviceRoleId;

            _mockAccreditationService.Setup(x => x.GetAccreditation(It.IsAny<Guid>()))
                .ReturnsAsync(new AccreditationDto
                {
                    ExternalId = accreditationId,
                    PrnTonnage = 500,
                    ApplicationTypeId = (int)ApplicationType.Reprocessor
                });

            _mockAccreditationService.Setup(x => x.GetAccreditationPrnIssueAuths(It.IsAny<Guid>()))
                .ReturnsAsync(
                    [
                        new AccreditationPrnIssueAuthDto
                        {
                            PersonExternalId = personId,
                            AccreditationExternalId = accreditationId
                        }
                    ]
                );

            // Act
            var result = await _controller.TaskList(accreditationId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            var model = viewResult.ViewData.Model as TaskListViewModel;
            Assert.IsNotNull(model);
            Assert.IsFalse(model.PeopleCanSubmitApplication.ApprovedPersons.Any());
        }

        [TestMethod]
        public async Task TaskList_ReturnsViewResult_TonnageAndAuthorityToIssuePrnStatus_NotStarted()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var personId = Guid.NewGuid();
            _userData.ServiceRoleId = (int)ServiceRole.Basic;
            var usersApproved = new List<UserModel>
            {
                new() { FirstName = "Joseph", LastName = "Bloggs" }
            };
            _mockAccountServiceClient.Setup(x => x.GetUsersForOrganisationAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(usersApproved);

            _mockAccreditationService.Setup(x => x.GetAccreditation(It.IsAny<Guid>()))
                 .ReturnsAsync(new AccreditationDto
                 {
                     ExternalId = accreditationId,

                     ApplicationTypeId = (int)ApplicationType.Reprocessor
                 });

            _mockAccreditationService.Setup(x => x.GetAccreditationPrnIssueAuths(It.IsAny<Guid>()))
                .ReturnsAsync((List<AccreditationPrnIssueAuthDto>)null);

            var routeMetadata = new EndpointMetadataCollection(new RouteNameMetadata(AccreditationController.RouteIds.AccreditationTaskList));
            var endPoint = new RouteEndpoint(
                requestDelegate: (ctx) => Task.CompletedTask,
                routePattern: RoutePatternFactory.Parse("/test"),
                order: 0,
                metadata: routeMetadata,
                displayName: null);

            _controller.HttpContext.SetEndpoint(endPoint);

            // Act
            var result = await _controller.TaskList(accreditationId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            var model = viewResult.ViewData.Model as TaskListViewModel;
            Assert.IsNotNull(model);
            model.Accreditation.ExternalId.Should().Be(accreditationId);
            model.ApplicationTypeDescription.Should().Be("PRN");
            model.PrnTonnageRouteName.Should().Be(RouteIds.SelectPrnTonnage);
            model.TonnageAndAuthorityToIssuePrnStatus.Should().Be(TaskStatus.NotStart);
        }

        [TestMethod]
        public async Task TaskList_ReturnsViewResult_TonnageAndAuthorityToIssuePrnStatus_InProgress()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var personId = Guid.NewGuid();
            _userData.ServiceRoleId = (int)ServiceRole.Basic;
            var usersApproved = new List<UserModel>
            {
                new() { FirstName = "Joseph", LastName = "Bloggs" }
            };
            _mockAccountServiceClient.Setup(x => x.GetUsersForOrganisationAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(usersApproved);

            _mockAccreditationService.Setup(x => x.GetAccreditation(It.IsAny<Guid>()))
                .ReturnsAsync(new AccreditationDto
                {
                    ExternalId = accreditationId,
                    PrnTonnage = 500,
                    PrnTonnageAndAuthoritiesConfirmed = false,
                    ApplicationTypeId = (int)ApplicationType.Reprocessor
                });

            _mockAccreditationService.Setup(x => x.GetAccreditationPrnIssueAuths(It.IsAny<Guid>()))
                .ReturnsAsync((List<AccreditationPrnIssueAuthDto>)null);

            // Act
            var result = await _controller.TaskList(accreditationId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            var model = viewResult.ViewData.Model as TaskListViewModel;
            Assert.IsNotNull(model);
            model.Accreditation.ExternalId.Should().Be(accreditationId);
            model.ApplicationTypeDescription.Should().Be("PRN");
            model.PrnTonnageRouteName.Should().Be(RouteIds.SelectPernTonnage);
            model.TonnageAndAuthorityToIssuePrnStatus.Should().Be(TaskStatus.InProgress);
        }

        [TestMethod]
        public async Task TaskList_ReturnsViewResult_TonnageAndAuthorityToIssuePrnStatus_Is_Completed()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var personId = Guid.NewGuid();
            _userData.ServiceRoleId = (int)ServiceRole.Basic;
            var usersApproved = new List<UserModel>
            {
                new() { FirstName = "Joseph", LastName = "Bloggs" }
            };
            _mockAccountServiceClient.Setup(x => x.GetUsersForOrganisationAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(usersApproved);

            _mockAccreditationService.Setup(x => x.GetAccreditation(It.IsAny<Guid>()))
                .ReturnsAsync(new AccreditationDto
                {
                    ExternalId = accreditationId,
                    PrnTonnage = 500,
                    PrnTonnageAndAuthoritiesConfirmed = true,
                    ApplicationTypeId = (int)ApplicationType.Reprocessor
                });

            _mockAccreditationService.Setup(x => x.GetAccreditationPrnIssueAuths(It.IsAny<Guid>()))
                .ReturnsAsync(
                    [
                        new AccreditationPrnIssueAuthDto
                        {
                            PersonExternalId = personId,
                            AccreditationExternalId = accreditationId
                        }
                    ]
                );

            // Act
            var result = await _controller.TaskList(accreditationId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            var model = viewResult.ViewData.Model as TaskListViewModel;
            Assert.IsNotNull(model);
            model.TonnageAndAuthorityToIssuePrnStatus.Should().Be(TaskStatus.Completed);
            model.BusinessPlanStatus.Should().Be(TaskStatus.NotStart);
        }

        [TestMethod]
        public async Task TaskList_ReturnsViewResult_BusinessPlanStatus_NotStarted()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            _mockAccreditationService.Setup(x => x.GetAccreditation(It.IsAny<Guid>()))
                .ReturnsAsync(new AccreditationDto
                {
                    ExternalId = accreditationId,
                    ApplicationTypeId = (int)ApplicationType.Reprocessor,
                    InfrastructurePercentage = null,
                    PackagingWastePercentage = null,
                    BusinessCollectionsPercentage = null,
                    CommunicationsPercentage = null,
                    NewMarketsPercentage = null,
                    NewUsesPercentage = null,
                    OtherPercentage = null,
                    BusinessPlanConfirmed = false
                });

            // Act
            var result = await _controller.TaskList(accreditationId);

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult.ViewData.Model as TaskListViewModel;
            model.BusinessPlanStatus.Should().Be(TaskStatus.NotStart);
        }

        [TestMethod]
        public async Task TaskList_ReturnsViewResult_BusinessPlanStatus_InProgress()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            _mockAccreditationService.Setup(x => x.GetAccreditation(It.IsAny<Guid>()))
                .ReturnsAsync(new AccreditationDto
                {
                    ExternalId = accreditationId,
                    ApplicationTypeId = (int)ApplicationType.Reprocessor,
                    InfrastructurePercentage = 50, // Value provided
                    PackagingWastePercentage = 50, // Value provided
                    BusinessCollectionsPercentage = null,
                    CommunicationsPercentage = null,
                    NewMarketsPercentage = null,
                    NewUsesPercentage = null,
                    OtherPercentage = null,
                    BusinessPlanConfirmed = false
                });

            // Act
            var result = await _controller.TaskList(accreditationId);

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult.ViewData.Model as TaskListViewModel;
            model.BusinessPlanStatus.Should().Be(TaskStatus.InProgress);
        }

        [TestMethod]
        public async Task TaskList_ReturnsViewResult_BusinessPlanStatus_Completed()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            _mockAccreditationService.Setup(x => x.GetAccreditation(It.IsAny<Guid>()))
                .ReturnsAsync(new AccreditationDto
                {
                    ExternalId = accreditationId,
                    ApplicationTypeId = (int)ApplicationType.Reprocessor,
                    BusinessPlanConfirmed = true
                });

            // Act
            var result = await _controller.TaskList(accreditationId);

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult.ViewData.Model as TaskListViewModel;
            model.BusinessPlanStatus.Should().Be(TaskStatus.Completed);
        }

        [TestMethod]
        public async Task TaskList_ReturnsViewResult_AccreditationSamplingAndInspectionPlanStatus_Completed()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var submissionId = Guid.NewGuid();
            var fileUploadExternalId = Guid.NewGuid();
            var fileId = Guid.NewGuid();
            var fileName = "file.csv";

            var accreditationFileUploadDtos = new List<AccreditationFileUploadDto>
            {
                new()
                {
                    SubmissionId = submissionId,
                    ExternalId = fileUploadExternalId,
                    FileId = fileId,
                    Filename = fileName,
                }
            };

            _mockAccreditationService.Setup(x => x.GetAccreditation(It.IsAny<Guid>()))
                .ReturnsAsync(new AccreditationDto
                {
                    ExternalId = accreditationId,
                    ApplicationTypeId = (int)ApplicationType.Reprocessor,
                    BusinessPlanConfirmed = true
                });

            _mockAccreditationService
                .Setup(s => s.GetAccreditationFileUploads(accreditationId, (int)AccreditationFileUploadType.SamplingAndInspectionPlan, (int)AccreditationFileUploadStatus.UploadComplete))
                .ReturnsAsync(accreditationFileUploadDtos);

            // Act
            var result = await _controller.TaskList(accreditationId);

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult.ViewData.Model as TaskListViewModel;
            model.AccreditationSamplingAndInspectionPlanStatus.Should().Be(TaskStatus.Completed);
        }
        #endregion

        #region ReviewBusinessPlan

        [TestMethod]
        public async Task ReviewBusinessPlan_Get_ReturnsViewResult()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var accreditation = new AccreditationDto
            {
                ExternalId = accreditationId,
                ApplicationTypeId = (int)ApplicationType.Reprocessor,
                InfrastructurePercentage = 1,
                PackagingWastePercentage = 2,
                BusinessCollectionsPercentage = 3,
                CommunicationsPercentage = 4,
                NewMarketsPercentage = 5,
                NewUsesPercentage = 6,
                OtherPercentage = 7,
                InfrastructureNotes = "Infra note",
                PackagingWasteNotes = "Price support",
                BusinessCollectionsNotes = "Biz note",
                CommunicationsNotes = "Comms note",
                NewMarketsNotes = "Market note",
                NewUsesNotes = "New uses note",
                OtherNotes = "Other note"
            };

            _mockAccreditationService
                .Setup(x => x.GetAccreditation(accreditationId))
                .ReturnsAsync(accreditation);

            // Act
            var result = await _controller.ReviewBusinessPlan(accreditationId);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ReviewBusinessPlanViewModel;
            model.Should().NotBeNull();

            model.AccreditationId.Should().Be(accreditation.ExternalId);
            model.ApplicationTypeId.Should().Be(accreditation.ApplicationTypeId);
            model.Subject.Should().Be("PRN");
            model.InfrastructurePercentage.Should().Be(accreditation.InfrastructurePercentage);
            model.PriceSupportPercentage.Should().Be(accreditation.PackagingWastePercentage);
            model.BusinessCollectionsPercentage.Should().Be(accreditation.BusinessCollectionsPercentage);
            model.CommunicationsPercentage.Should().Be(accreditation.CommunicationsPercentage);
            model.NewMarketsPercentage.Should().Be(accreditation.NewMarketsPercentage);
            model.NewUsesPercentage.Should().Be(accreditation.NewUsesPercentage);
            model.OtherPercentage.Should().Be(accreditation.OtherPercentage);
            model.InfrastructureNotes.Should().Be(accreditation.InfrastructureNotes);
            model.PriceSupportNotes.Should().Be(accreditation.PackagingWasteNotes);
            model.BusinessCollectionsNotes.Should().Be(accreditation.BusinessCollectionsNotes);
            model.CommunicationsNotes.Should().Be(accreditation.CommunicationsNotes);
            model.NewMarketsNotes.Should().Be(accreditation.NewMarketsNotes);
            model.NewUsesNotes.Should().Be(accreditation.NewUsesNotes);
            model.OtherNotes.Should().Be(accreditation.OtherNotes);
        }

        [TestMethod]
        public async Task ReviewBusinessPlan_Post_ActionIsContinueAndTypeIsReprocessor_UpdatesBusinessPlanStatusToCompleted()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var accreditation = new AccreditationDto
            {
                ExternalId = accreditationId,
                ApplicationTypeId = (int)ApplicationType.Reprocessor,
            };

            _mockAccreditationService
                .Setup(x => x.GetAccreditation(accreditationId))
                .ReturnsAsync(accreditation);

            var viewModel = new ReviewBusinessPlanViewModel { AccreditationId = accreditationId, ApplicationTypeId = (int)ApplicationType.Reprocessor, Action = "continue" };

            // Act
            var result = await _controller.ReviewBusinessPlan(viewModel);

            // Assert
            _mockAccreditationService.Verify(s => s.UpsertAccreditation(It.Is<AccreditationRequestDto>(dto =>
                dto.ExternalId == accreditationId &&
                dto.BusinessPlanConfirmed == true
            )), Times.Once);

            result.Should().BeOfType<RedirectToRouteResult>();
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.AccreditationTaskList);
            redirectResult.RouteValues.Count.Should().Be(1);
            redirectResult.RouteValues["AccreditationId"].Should().Be(viewModel.AccreditationId);
        }

        [TestMethod]
        public async Task ReviewBusinessPlan_Post_ActionIsContinueAndTypeIsExporter_UpdatesBusinessPlanStatusToCompleted()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var accreditation = new AccreditationDto
            {
                ExternalId = accreditationId,
                ApplicationTypeId = (int)ApplicationType.Exporter,
            };

            _mockAccreditationService
                .Setup(x => x.GetAccreditation(accreditationId))
                .ReturnsAsync(accreditation);

            var viewModel = new ReviewBusinessPlanViewModel { AccreditationId = accreditationId, ApplicationTypeId = (int)ApplicationType.Exporter, Action = "continue" };

            // Act
            var result = await _controller.ReviewBusinessPlan(viewModel);

            // Assert
            _mockAccreditationService.Verify(s => s.UpsertAccreditation(It.Is<AccreditationRequestDto>(dto =>
                dto.ExternalId == accreditationId &&
                dto.BusinessPlanConfirmed == true
            )), Times.Once);

            result.Should().BeOfType<RedirectToRouteResult>();
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.ExporterAccreditationTaskList);
            redirectResult.RouteValues.Count.Should().Be(1);
            redirectResult.RouteValues["AccreditationId"].Should().Be(viewModel.AccreditationId);
        }

        [TestMethod]
        public async Task ReviewBusinessPlan_Post_ActionIsSave_ReturnsRedirectToApplicationSaved()
        {
            // Arrange
            var viewModel = new ReviewBusinessPlanViewModel { Action = "save" };

            // Act
            var result = await _controller.ReviewBusinessPlan(viewModel);

            // Assert
            _mockAccreditationService.Verify(s => s.UpsertAccreditation(It.IsAny<AccreditationRequestDto>()), Times.Never);

            result.Should().BeOfType<RedirectToRouteResult>();
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.ApplicationSaved);
        }

        [TestMethod]
        public async Task ReviewBusinessPlan_Post_ActionIsUnknown_ReturnsBadRequest()
        {
            // Arrange
            var viewModel = new ReviewBusinessPlanViewModel { Action = "unknown" };

            // Act
            var result = await _controller.ReviewBusinessPlan(viewModel);

            // Assert
            _mockAccreditationService.Verify(s => s.UpsertAccreditation(It.IsAny<AccreditationRequestDto>()), Times.Never);

            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.Value.Should().Be("Invalid action supplied.");
        }

        #endregion

        #region SamplingAndInspectionPlan

        [TestMethod]
        public async Task SamplingAndInspectionPlan_Get_ReturnsView()
        {
            // Arrange
            SetupTempData(_controller);
            var accreditationId = Guid.NewGuid();
            var submissionId = Guid.NewGuid();
            var fileUploadExternalId = Guid.NewGuid();
            var fileId = Guid.NewGuid();
            var fileName = "file.csv";

            var accreditation = new AccreditationDto
            {
                ExternalId = accreditationId,
                ApplicationTypeId = (int)ApplicationType.Reprocessor,
                InfrastructurePercentage = 1,
                PackagingWastePercentage = 2,
                BusinessCollectionsPercentage = 3,
                CommunicationsPercentage = 4,
                NewMarketsPercentage = 5,
                NewUsesPercentage = 6,
                OtherPercentage = 7,
                InfrastructureNotes = "Infra note",
                PackagingWasteNotes = "Price support",
                BusinessCollectionsNotes = "Biz note",
                CommunicationsNotes = "Comms note",
                NewMarketsNotes = "Market note",
                NewUsesNotes = "New uses note",
                OtherNotes = "Other note"
            };

            var accreditationFileUploadDtos = new List<AccreditationFileUploadDto>
            {
                new()
                {
                    SubmissionId = submissionId,
                    ExternalId = fileUploadExternalId,
                    FileId = fileId,
                    Filename = fileName,
                }
            };

            _mockAccreditationService
                .Setup(x => x.GetAccreditation(accreditationId))
                .ReturnsAsync(accreditation);

            _mockAccreditationService
                .Setup(s => s.GetAccreditationFileUploads(accreditationId,(int)AccreditationFileUploadType.SamplingAndInspectionPlan, (int)AccreditationFileUploadStatus.UploadComplete))
                .ReturnsAsync(accreditationFileUploadDtos);

            var backUrl = $"/epr-prn/accreditation/reprocessor-accreditation-task-list/{accreditationId}";
            _mockUrlHelperMock.Setup(u => u.RouteUrl(It.IsAny<UrlRouteContext>()))
                .Returns(backUrl);

            // Act
            var result = await _controller.SamplingAndInspectionPlan(accreditationId);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as SamplingAndInspectionPlanViewModel;
            model.Should().NotBeNull();
            model.AccreditationId.Should().Be(accreditationId);
            model.ApplicationTypeId.Should().Be(accreditation.ApplicationTypeId);
            model.SuccessBanner.Should().BeNull();
            model.UploadedFiles.Count.Should().Be(accreditationFileUploadDtos.Count);
            model.AccreditationId.Should().Be(accreditationId);

            var backlink = _controller.ViewBag.BackLinkToDisplay as string;
            backlink.Should().Be(backUrl);
        }

        [TestMethod]
        public async Task SamplingAndInspectionPlan_When_NoUploadedFiles_Get_ReturnsView()
        {
            // Arrange
            SetupTempData(_controller);
            var accreditationId = Guid.NewGuid();
            var submissionId = Guid.NewGuid();
            var fileUploadExternalId = Guid.NewGuid();
            var fileId = Guid.NewGuid();
            var fileName = "file.csv";

            var accreditation = new AccreditationDto
            {
                ExternalId = accreditationId,
                ApplicationTypeId = (int)ApplicationType.Reprocessor,
                InfrastructurePercentage = 1,
                PackagingWastePercentage = 2,
                BusinessCollectionsPercentage = 3,
                CommunicationsPercentage = 4,
                NewMarketsPercentage = 5,
                NewUsesPercentage = 6,
                OtherPercentage = 7,
                InfrastructureNotes = "Infra note",
                PackagingWasteNotes = "Price support",
                BusinessCollectionsNotes = "Biz note",
                CommunicationsNotes = "Comms note",
                NewMarketsNotes = "Market note",
                NewUsesNotes = "New uses note",
                OtherNotes = "Other note"
            };

            List<AccreditationFileUploadDto> accreditationFileUploadDtos = null;

            _mockAccreditationService
                .Setup(x => x.GetAccreditation(accreditationId))
                .ReturnsAsync(accreditation);

            _mockAccreditationService
                .Setup(s => s.GetAccreditationFileUploads(accreditationId, (int)AccreditationFileUploadType.SamplingAndInspectionPlan, (int)AccreditationFileUploadStatus.UploadComplete))
                .ReturnsAsync(accreditationFileUploadDtos);

            var backUrl = $"/epr-prn/accreditation/reprocessor-accreditation-task-list/{accreditationId}";
            _mockUrlHelperMock.Setup(u => u.RouteUrl(It.IsAny<UrlRouteContext>()))
                .Returns(backUrl);

            // Act
            var result = await _controller.SamplingAndInspectionPlan(accreditationId);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as SamplingAndInspectionPlanViewModel;
            model.Should().NotBeNull();
            model.AccreditationId.Should().Be(accreditationId);
            model.ApplicationTypeId.Should().Be(accreditation.ApplicationTypeId);
            model.SuccessBanner.Should().BeNull();
            model.UploadedFiles.Should().BeNull();
            model.AccreditationId.Should().Be(accreditationId);

            var backlink = _controller.ViewBag.BackLinkToDisplay as string;
            backlink.Should().Be(backUrl);
        }

        [TestMethod]
        public async Task SamplingAndInspectionPlan_Get_When_Submission_HasErrors_ReturnsView()
        {
            // Arrange
            SetupTempData(_controller);
            var accreditationId = Guid.NewGuid();
            var submissionId = Guid.NewGuid();
            var fileUploadExternalId = Guid.NewGuid();
            var fileId = Guid.NewGuid();
            var fileName = "file.csv";

            var accreditation = new AccreditationDto
            {
                ExternalId = accreditationId,
                ApplicationTypeId = (int)ApplicationType.Reprocessor,
            };

            var accreditationFileUploadDtos = new List<AccreditationFileUploadDto>
            {
                new()
                {
                    SubmissionId = submissionId,
                    ExternalId = fileUploadExternalId,
                    FileId = fileId,
                    Filename = fileName,
                }
            };

            _mockAccreditationService
                .Setup(x => x.GetAccreditation(accreditationId))
                .ReturnsAsync(accreditation);

            _mockAccreditationService
                .Setup(s => s.GetAccreditationFileUploads(accreditationId, (int)AccreditationFileUploadType.SamplingAndInspectionPlan, (int)AccreditationFileUploadStatus.UploadComplete))
                .ReturnsAsync(accreditationFileUploadDtos);


            var accreditationSubmission = new AccreditationSubmission
            {
                AccreditationDataComplete = true,
                Errors = new List<string>() { "81" }
            };

            _mockFileUploadService.Setup(s => s.GetFileUploadSubmissionStatusAsync<AccreditationSubmission>(submissionId)).ReturnsAsync(accreditationSubmission);


            var backUrl = $"/epr-prn/accreditation/reprocessor-accreditation-task-list/{accreditationId}";
            _mockUrlHelperMock.Setup(u => u.RouteUrl(It.IsAny<UrlRouteContext>()))
                .Returns(backUrl);

            // Act
            var result = await _controller.SamplingAndInspectionPlan(accreditationId, submissionId);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as SamplingAndInspectionPlanViewModel;
            model.Should().NotBeNull();
            model.AccreditationId.Should().Be(accreditationId);
            model.ApplicationTypeId.Should().Be(accreditation.ApplicationTypeId);
            model.SuccessBanner.Should().BeNull();
            model.UploadedFiles.Count.Should().Be(accreditationFileUploadDtos.Count);
            model.AccreditationId.Should().Be(accreditationId);

            var backlink = _controller.ViewBag.BackLinkToDisplay as string;
            backlink.Should().Be(backUrl);

            Assert.IsTrue(_controller.ModelState.Values.SelectMany(v => v.Errors).Any(e => e.ErrorMessage.Equals("The selected file contains a virus")), "Validation error messages check");
        }

        [TestMethod]
        public async Task SamplingAndInspectionPlan_Get_When_Submission_HasNoErrors_Should_Insert_FileUpload()
        {
            // Arrange
            SetupTempData(_controller);
            var accreditationId = Guid.NewGuid();
            var submissionId = Guid.NewGuid();
            var fileUploadExternalId = Guid.NewGuid();
            var fileId = Guid.NewGuid();
            var fileName = "file.csv";

            var accreditation = new AccreditationDto
            {
                ExternalId = accreditationId,
                ApplicationTypeId = (int)ApplicationType.Reprocessor,
            };

            var accreditationFileUploadDtos = new List<AccreditationFileUploadDto>();

            _mockAccreditationService
                .Setup(x => x.GetAccreditation(accreditationId))
                .ReturnsAsync(accreditation);

            _mockAccreditationService
                .Setup(s => s.GetAccreditationFileUploads(accreditationId, (int)AccreditationFileUploadType.SamplingAndInspectionPlan, (int)AccreditationFileUploadStatus.UploadComplete))
                .ReturnsAsync(accreditationFileUploadDtos);

            var accreditationSubmission = new AccreditationSubmission
            {
                AccreditationFileName = fileName,
                FileId = fileId,
                AccreditationFileUploadDateTime = DateTime.UtcNow,
                AccreditationDataComplete = true,
                Errors = new List<string>()
            };

            _mockFileUploadService.Setup(s => s.GetFileUploadSubmissionStatusAsync<AccreditationSubmission>(submissionId)).ReturnsAsync(accreditationSubmission);


            var backUrl = $"/epr-prn/accreditation/reprocessor-accreditation-task-list/{accreditationId}";
            _mockUrlHelperMock.Setup(u => u.RouteUrl(It.IsAny<UrlRouteContext>()))
                .Returns(backUrl);

            // Act
            var result = await _controller.SamplingAndInspectionPlan(accreditationId, submissionId);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as SamplingAndInspectionPlanViewModel;
            model.Should().NotBeNull();
            model.AccreditationId.Should().Be(accreditationId);
            model.ApplicationTypeId.Should().Be(accreditation.ApplicationTypeId);
            model.SuccessBanner.Should().BeNull();
            model.UploadedFiles.Count.Should().Be(accreditationFileUploadDtos.Count);
            model.AccreditationId.Should().Be(accreditationId);

            _mockAccreditationService.Verify(s =>
                    s.UpsertAccreditationFileUpload(
                        accreditationId,
                        It.IsAny<AccreditationFileUploadDto>()), Times.Once);

            var backlink = _controller.ViewBag.BackLinkToDisplay as string;
            backlink.Should().Be(backUrl);
        }

        [TestMethod]
        public async Task SamplingAndInspectionPlan_Get_When_Submission_HasNoErrors_ReturnsView()
        {
            // Arrange
            SetupTempData(_controller);
            var accreditationId = Guid.NewGuid();
            var submissionId = Guid.NewGuid();
            var fileUploadExternalId = Guid.NewGuid();
            var fileId = Guid.NewGuid();
            var fileName = "file.csv";

            var accreditation = new AccreditationDto
            {
                ExternalId = accreditationId,
                ApplicationTypeId = (int)ApplicationType.Reprocessor,
            };

            var accreditationFileUploadDtos = new List<AccreditationFileUploadDto>
            {
                new()
                {
                    SubmissionId = submissionId,
                    ExternalId = fileUploadExternalId,
                    FileId = fileId,
                    Filename = fileName,
                }
            };

            _mockAccreditationService
                .Setup(x => x.GetAccreditation(accreditationId))
                .ReturnsAsync(accreditation);

            _mockAccreditationService
                .Setup(s => s.GetAccreditationFileUploads(accreditationId, (int)AccreditationFileUploadType.SamplingAndInspectionPlan, (int)AccreditationFileUploadStatus.UploadComplete))
                .ReturnsAsync(accreditationFileUploadDtos);

            var accreditationSubmission = new AccreditationSubmission
            {
                AccreditationFileName = fileName,
                FileId = fileId,
                AccreditationFileUploadDateTime = DateTime.UtcNow,
                AccreditationDataComplete = true,
                Errors = new List<string>()
            };

            _mockFileUploadService.Setup(s => s.GetFileUploadSubmissionStatusAsync<AccreditationSubmission>(submissionId)).ReturnsAsync(accreditationSubmission);


            var backUrl = $"/epr-prn/accreditation/reprocessor-accreditation-task-list/{accreditationId}";
            _mockUrlHelperMock.Setup(u => u.RouteUrl(It.IsAny<UrlRouteContext>()))
                .Returns(backUrl);

            // Act
            var result = await _controller.SamplingAndInspectionPlan(accreditationId, submissionId);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as SamplingAndInspectionPlanViewModel;
            model.Should().NotBeNull();
            model.AccreditationId.Should().Be(accreditationId);
            model.ApplicationTypeId.Should().Be(accreditation.ApplicationTypeId);
            model.SuccessBanner.Should().BeNull();
            model.UploadedFiles.Count.Should().Be(accreditationFileUploadDtos.Count);
            model.AccreditationId.Should().Be(accreditationId);

            _mockAccreditationService.Verify(s =>
                    s.UpsertAccreditationFileUpload(
                        accreditationId,
                        It.IsAny<AccreditationFileUploadDto>()), Times.Never);

            var backlink = _controller.ViewBag.BackLinkToDisplay as string;
            backlink.Should().Be(backUrl);
        }

        [TestMethod]
        public async Task SamplingAndInspectionPlan_Should_Show_SuccessBanner()
        {
            // Arrange
            var notificationBanner = new NotificationBannerModel
            {
                Message = "Success"
            };

            var tempData = new Dictionary<string, object>
            {
                { Constants.AccreditationFileDeletedNotification, System.Text.Json.JsonSerializer.Serialize(notificationBanner) }
            };

            SetupTempData(_controller, tempData);
            var accreditationId = Guid.NewGuid();
            var submissionId = Guid.NewGuid();
            var fileUploadExternalId = Guid.NewGuid();
            var fileId = Guid.NewGuid();
            var fileName = "file.csv";

            var accreditation = new AccreditationDto
            {
                ExternalId = accreditationId,
                ApplicationTypeId = (int)ApplicationType.Reprocessor,
            };

            var accreditationFileUploadDtos = new List<AccreditationFileUploadDto>
            {
                new()
                {
                    SubmissionId = submissionId,
                    ExternalId = fileUploadExternalId,
                    FileId = fileId,
                    Filename = fileName,
                }
            };

            _mockAccreditationService
                .Setup(x => x.GetAccreditation(accreditationId))
                .ReturnsAsync(accreditation);

            _mockAccreditationService
                .Setup(s => s.GetAccreditationFileUploads(accreditationId, (int)AccreditationFileUploadType.SamplingAndInspectionPlan, (int)AccreditationFileUploadStatus.UploadComplete))
                .ReturnsAsync(accreditationFileUploadDtos);

            var accreditationSubmission = new AccreditationSubmission
            {
                AccreditationFileName = fileName,
                FileId = fileId,
                AccreditationFileUploadDateTime = DateTime.UtcNow,
                AccreditationDataComplete = true,
                Errors = new List<string>()
            };

            _mockFileUploadService.Setup(s => s.GetFileUploadSubmissionStatusAsync<AccreditationSubmission>(submissionId)).ReturnsAsync(accreditationSubmission);


            var backUrl = $"/epr-prn/accreditation/reprocessor-accreditation-task-list/{accreditationId}";
            _mockUrlHelperMock.Setup(u => u.RouteUrl(It.IsAny<UrlRouteContext>()))
                .Returns(backUrl);

            // Act
            var result = await _controller.SamplingAndInspectionPlan(accreditationId, submissionId);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as SamplingAndInspectionPlanViewModel;
            model.Should().NotBeNull();
            model.AccreditationId.Should().Be(accreditationId);
            model.ApplicationTypeId.Should().Be(accreditation.ApplicationTypeId);
            model.SuccessBanner.Should().BeEquivalentTo(notificationBanner);
            model.UploadedFiles.Count.Should().Be(accreditationFileUploadDtos.Count);
            model.AccreditationId.Should().Be(accreditationId);

            _mockAccreditationService.Verify(s =>
                    s.UpsertAccreditationFileUpload(
                        accreditationId,
                        It.IsAny<AccreditationFileUploadDto>()), Times.Never);

            var backlink = _controller.ViewBag.BackLinkToDisplay as string;
            backlink.Should().Be(backUrl);
        }

        [TestMethod]
        public async Task SamplingAndInspectionPlan_Post_Upload_Action_InValid_ModalState_ReturnsSameView()
        {
            // Arrange
            SetupTempData(_controller);
            var accreditationId = Guid.NewGuid();

            var model = new SamplingAndInspectionPlanViewModel
            {
                File = null,
                Action = "upload"
            };
            
            // Act
            var result = await _controller.SamplingAndInspectionPlan(model);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var samplingViewModel = viewResult.Model as SamplingAndInspectionPlanViewModel;
            samplingViewModel.UploadedFiles.Should().BeNull();
            Assert.IsTrue(_controller.ModelState.Values.SelectMany(v => v.Errors).Any(e => e.ErrorMessage.Equals("The selected file is empty")), "Validation error messages check");
        }

        [TestMethod]
        public async Task SamplingAndInspectionPlan_Post_Upload_Action_InValid_ModalState_ReturnsView_WithUploadedFiles()
        {
            // Arrange
            SetupTempData(_controller);
            var accreditationId = Guid.NewGuid();
            var submissionId = Guid.NewGuid();
            var fileUploadExternalId = Guid.NewGuid();
            var fileId = Guid.NewGuid();
            var fileName = "file.csv";

            var accreditationFileUploadDtos = new List<AccreditationFileUploadDto>
            {
                new()
                {
                    SubmissionId = submissionId,
                    ExternalId = fileUploadExternalId,
                    FileId = fileId,
                    Filename = fileName,
                }
            };

            var model = new SamplingAndInspectionPlanViewModel
            {
                AccreditationId = accreditationId,
                File = null,
                Action = "upload"
            };

            _mockAccreditationService
                .Setup(s => s.GetAccreditationFileUploads(accreditationId, (int)AccreditationFileUploadType.SamplingAndInspectionPlan, (int)AccreditationFileUploadStatus.UploadComplete))
                .ReturnsAsync(accreditationFileUploadDtos);


            // Act
            var result = await _controller.SamplingAndInspectionPlan(model);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var samplingViewModel = viewResult.Model as SamplingAndInspectionPlanViewModel;
            samplingViewModel.UploadedFiles.Should().NotBeNull();
            samplingViewModel.UploadedFiles.Count.Should().Be(accreditationFileUploadDtos.Count);
            Assert.IsTrue(_controller.ModelState.Values.SelectMany(v => v.Errors).Any(e => e.ErrorMessage.Equals("The selected file is empty")), "Validation error messages check");
        }

        [TestMethod]
        public async Task SamplingAndInspectionPlan_Post_Upload_Action_Valid_ModalState_RedirectsToUploadingPage()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var submissionId = Guid.NewGuid();
            var content = new byte[100];
            using var stream = new MemoryStream(content);
            var formFile = new FormFile(stream, 0, 1024, "Test data", "file.csv");

            var model = new SamplingAndInspectionPlanViewModel
            {
                AccreditationId = accreditationId,
                File = formFile,
                Action = "upload"
            };

            _mockFileUploadService
                .Setup(s => s.UploadFileAccreditationAsync(
                    It.IsAny<byte[]>(),
                    formFile.FileName,
                    SubmissionType.Accreditation,
                    It.IsAny<Guid?>()))
                .ReturnsAsync(submissionId);

            // Act
            var result = await _controller.SamplingAndInspectionPlan(model);

            // Assert
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.AccreditationUploadingAndValidatingFile);
            redirectResult.RouteValues["AccreditationId"].Should().Be(accreditationId);
            redirectResult.RouteValues["SubmissionId"].Should().Be(submissionId);
        }

        [TestMethod]
        public async Task SamplingAndInspectionPlan_Post_Continue_Action_NoFileUploads_Exists_ReturnsSameView()
        {
            // Arrange
            SetupTempData(_controller);
            var accreditationId = Guid.NewGuid();

            var model = new SamplingAndInspectionPlanViewModel
            {
                Action = "continue"
            };

            List<AccreditationFileUploadDto> accreditationFileUploadDtos = null;

            _mockAccreditationService
                .Setup(s => s.GetAccreditationFileUploads(accreditationId, (int)AccreditationFileUploadType.SamplingAndInspectionPlan, (int)AccreditationFileUploadStatus.UploadComplete))
                .ReturnsAsync(accreditationFileUploadDtos);

            // Act
            var result = await _controller.SamplingAndInspectionPlan(model);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().Be(model);
            Assert.IsTrue(_controller.ModelState.Values.SelectMany(v => v.Errors).Any(e => e.ErrorMessage.Equals("Select a sampling and inspection plan")), "Validation error messages check");
        }

        [TestMethod]
        public async Task SamplingAndInspectionPlan_Post_Continue_Action_FileUploads_Exists_RedirectToTaskListPage()
        {
            // Arrange
            SetupTempData(_controller);
            var accreditationId = Guid.NewGuid();

            var model = new SamplingAndInspectionPlanViewModel
            {
                AccreditationId = accreditationId,
                ApplicationTypeId = (int)ApplicationType.Reprocessor,
                Action = "continue"
            };

            var accreditationFileUploadDtos = new List<AccreditationFileUploadDto>
            {
                new()
                {
                    SubmissionId = Guid.NewGuid(),
                    ExternalId = Guid.NewGuid(),
                    FileId = Guid.NewGuid(),
                    Filename = "fileName.csv",
                }
            };

            _mockAccreditationService
                .Setup(s => s.GetAccreditationFileUploads(accreditationId, (int)AccreditationFileUploadType.SamplingAndInspectionPlan, (int)AccreditationFileUploadStatus.UploadComplete))
                .ReturnsAsync(accreditationFileUploadDtos);

            // Act
            var result = await _controller.SamplingAndInspectionPlan(model);

            // Assert
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.AccreditationTaskList);
            redirectResult.RouteValues["AccreditationId"].Should().Be(accreditationId);
        }

        [TestMethod]
        public async Task SamplingAndInspectionPlan_Post_Save_Action_ValidModel_RedirectsToApplicationSaved()
        {
            // Arrange
            SetupTempData(_controller);

            var model = new SamplingAndInspectionPlanViewModel
            {
                Action = "save"
            };

            // Act
            var result = await _controller.SamplingAndInspectionPlan(model);

            // Assert
            result.Should().BeOfType<RedirectToRouteResult>();
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.ApplicationSaved);
        }

        [TestMethod]
        public async Task SamplingAndInspectionPlan_Post_Invalid_Action_ReturnsBadRequest()
        {
            // Arrange
            SetupTempData(_controller);

            var model = new SamplingAndInspectionPlanViewModel
            {
                Action = "invalid"
            };

            // Act
            var result = await _controller.SamplingAndInspectionPlan(model);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.Value.Should().Be("Invalid action supplied.");
        }

        #endregion

        #region ApplyingFor2026Accreditation

        [TestMethod]
        public void ApplyingFor2026Accreditation_ReturnsViewResult()
        {
            // Act
            var result = _controller.ApplyingFor2026Accreditation(new Guid());

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult), "Expected a ViewResult to be returned.");
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult, "Expected the ViewResult to not be null.");
        }

        #endregion

        #region Declaration

        [TestMethod]
        public async Task Declaration_Get_ReturnsViewWithModel()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var accreditation = new AccreditationDto
            {
                ExternalId = accreditationId,
                ApplicationTypeId = (int)ApplicationType.Reprocessor
            };
            _mockAccreditationService.Setup(s => s.GetAccreditation(accreditationId))
                .ReturnsAsync(accreditation);

            // Act
            var result = await _controller.Declaration(accreditationId);

            // Assert
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as DeclarationViewModel;
            model.Should().NotBeNull();
            model.AccreditationId.Should().Be(accreditationId);
            model.ApplicationTypeId.Should().Be((int)ApplicationType.Reprocessor);
            model.CompanyName.Should().Be("Some Organisation");
        }

        [TestMethod]
        public async Task Declaration_Post_InvalidViewModel_ReturnsSameView()
        {
            // Arrange
            var model = new DeclarationViewModel
            {
                AccreditationId = Guid.NewGuid(),
                ApplicationTypeId = (int)ApplicationType.Reprocessor
            };
            _controller.ModelState.AddModelError("FullName", "Required");

            // Act
            var result = await _controller.Declaration(model);

            // Assert
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().Be(model);
        }

        [TestMethod]
        public async Task Declaration_Post_ValidModel_UpdatesAccreditationAndRedirects()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var model = new DeclarationViewModel
            {
                AccreditationId = accreditationId,
                ApplicationTypeId = (int)ApplicationType.Reprocessor,
                FullName = "Test User",
                JobTitle = "Manager"
            };
            var accreditation = new AccreditationDto
            {
                ExternalId = accreditationId,
                ApplicationTypeId = (int)ApplicationType.Reprocessor
            };
            _mockAccreditationService.Setup(s => s.GetAccreditation(accreditationId)).ReturnsAsync(accreditation);
            _mockAccreditationService.Setup(s => s.CreateApplicationReferenceNumber(
                                                   It.IsAny<ApplicationType>(), It.IsAny<string>())).Returns("PR/PK/EXP-A123456");

            // Act
            var result = await _controller.Declaration(model);

            // Assert
            _mockAccreditationService.Verify(x => x.CreateApplicationReferenceNumber(
                                             It.IsAny<ApplicationType>(), It.IsAny<string>()), Times.Once);

            _mockAccreditationService.Verify(s => s.UpsertAccreditation(It.Is<AccreditationRequestDto>(dto =>
                dto.ExternalId == accreditationId &&
                dto.DecFullName == "Test User" &&
                dto.DecJobTitle == "Manager" &&
                dto.AccreditationStatusId == (int)Enums.AccreditationStatus.Submitted
            )), Times.Once);

            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.ReprocessorConfirmApplicationSubmission);
            redirectResult.RouteValues["AccreditationId"].Should().Be(accreditationId);
        }

        #endregion

        #region ConfirmApplicationSubmission
        [TestMethod]
        public async Task WhenApplicationReferenceNumberExists_ApplicationSubmissionConfirmation_ReturnsViewResult()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var applicationReferenceNumber = "/PK/EXP-A123456";
            var accreditation = new AccreditationDto
            {
                ExternalId = accreditationId,
                AccreferenceNumber = applicationReferenceNumber,
                MaterialName = "Steel"
            };
            _mockAccreditationService.Setup(s => s.GetAccreditation(accreditationId)).ReturnsAsync(accreditation);

            // Act
            var result = await _controller.ApplicationSubmissionConfirmation(accreditationId);

            // Assert
            _mockAccreditationService.Verify(x => x.CreateApplicationReferenceNumber(
                                             It.IsAny<ApplicationType>(), It.IsAny<string>()), Times.Never);

            var viewResult = result as ViewResult;
            var model = viewResult.Model as ApplicationSubmissionConfirmationViewModel;
            viewResult.Should().NotBeNull();
            model.Should().NotBeNull();
            model.ApplicationReferenceNumber.Should().Be(applicationReferenceNumber);
            model.MaterialName.Should().Be(accreditation.MaterialName.ToLower());
        }
        #endregion

        #region SelectOverseasSites

        [TestMethod]
        public async Task SelectOverseasSites_Get_ReturnsViewWithModel()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            SetupTempData(_controller);

            // Act
            var result = await _controller.SelectOverseasSites(accreditationId);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().BeOfType<SelectOverseasSitesViewModel>();
            var model = viewResult.Model as SelectOverseasSitesViewModel;
            model.Should().NotBeNull();
            model.AccreditationId.Should().Be(accreditationId);
            model.OverseasSites.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public async Task SelectOverseasSites_Post_ValidModel_ContinueAction_RedirectsToCheckOverseasSites()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            SetupTempData(_controller);

            var model = new SelectOverseasSitesViewModel
            {
                AccreditationId = accreditationId,
                OverseasSites = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>(),
                SelectedOverseasSites = new List<string> { "1", "2" },
                Action = "continue"
            };

            // Act
            var result = await _controller.SelectOverseasSites(model);

            // Assert
            result.Should().BeOfType<RedirectToRouteResult>();
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.CheckOverseasSites);
            redirectResult.RouteValues["accreditationId"].Should().Be(accreditationId);
        }

        [TestMethod]
        public async Task SelectOverseasSites_Post_ValidModel_SaveAction_RedirectsToApplicationSaved()
        {
            // Arrange
            SetupTempData(_controller);

            var model = new SelectOverseasSitesViewModel
            {
                AccreditationId = Guid.NewGuid(),
                OverseasSites = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>(),
                SelectedOverseasSites = new List<string> { "1" },
                Action = "save"
            };

            // Act
            var result = await _controller.SelectOverseasSites(model);

            // Assert
            result.Should().BeOfType<RedirectToRouteResult>();
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.ApplicationSaved);
        }

        [TestMethod]
        public async Task SelectOverseasSites_Post_InvalidAction_ReturnsBadRequest()
        {
            // Arrange
            SetupTempData(_controller);

            var model = new SelectOverseasSitesViewModel
            {
                AccreditationId = Guid.NewGuid(),
                OverseasSites = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>(),
                SelectedOverseasSites = new List<string> { "1" },
                Action = "invalid"
            };

            // Act
            var result = await _controller.SelectOverseasSites(model);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.Value.Should().Be("Invalid action supplied.");
        }

        [TestMethod]
        public async Task SelectOverseasSites_Post_NoSitesSelected_ReturnsViewWithError()
        {
            // Arrange
            var model = new SelectOverseasSitesViewModel
            {
                AccreditationId = Guid.NewGuid(),
                OverseasSites = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
                {
                    new() { Value = "1", Text = "ABC Exporters Ltd" },
                    new() { Value = "2", Text = "DEF Exporters Ltd" }
                },
                SelectedOverseasSites = new List<string>(),
                Action = "continue"
            };

            var validationContext = new ValidationContext(model);
            var validationResults = model.Validate(validationContext).ToList();
            foreach (var validationResult in validationResults)
            {
                _controller.ModelState.AddModelError(string.Empty, validationResult.ErrorMessage);
            }

            // Act
            var result = await _controller.SelectOverseasSites(model);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().Be(model);
            Assert.IsFalse(_controller.ModelState.IsValid, "ModelState should be invalid when no sites are selected.");
            Assert.IsTrue(_controller.ModelState.Values.SelectMany(v => v.Errors).Any(e => e.ErrorMessage.Equals("Select the overseas sites you want to accredit")), "Validation error messages check");
        }
        #endregion

        #region CheckOverseasSites

        [TestMethod]
        public void CheckOverseasSites_Get_WithData_ReturnsViewWithModel()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var model = new SelectOverseasSitesViewModel
            {
                AccreditationId = accreditationId,
                OverseasSites = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
                {
                    new() { Value = "1", Text = "ABC Exporters Ltd" },
                    new() { Value = "2", Text = "DEF Exporters Ltd" }
                },
                SelectedOverseasSites = new List<string> { "1" }
            };
            var tempData = new Dictionary<string, object>
            {
                { "SelectOverseasSitesModel", System.Text.Json.JsonSerializer.Serialize(model) }
            };
            SetupTempData(_controller, tempData);

            // Act
            var result = _controller.CheckOverseasSites(accreditationId);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var returnedModel = viewResult.Model as SelectOverseasSitesViewModel;
            returnedModel.Should().NotBeNull();
            returnedModel.AccreditationId.Should().Be(accreditationId);
            returnedModel.SelectedOverseasSites.Should().Contain("1");
        }

        [TestMethod]
        public void CheckOverseasSites_Get_WithoutTempData_RedirectsToSelectOverseasSites()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            SetupTempData(_controller);

            // Act
            var result = _controller.CheckOverseasSites(accreditationId);

            // Assert
            result.Should().BeOfType<RedirectToRouteResult>();
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.SelectOverseasSites);
            redirectResult.RouteValues["accreditationId"].Should().Be(accreditationId);
        }

        [TestMethod]
        public void CheckOverseasSites_Post_RemoveSite_UpdatesModelAndSetsData()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var model = new SelectOverseasSitesViewModel
            {
                AccreditationId = accreditationId,
                OverseasSites = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
                {
                    new() { Value = "1", Text = "ABC Exporters Ltd" },
                    new() { Value = "2", Text = "DEF Exporters Ltd" }
                },
                SelectedOverseasSites = new List<string> { "1", "2" }
            };
            var tempData = new Dictionary<string, object>
            {
                { "SelectOverseasSitesModel", System.Text.Json.JsonSerializer.Serialize(model) }
            };
            SetupTempData(_controller, tempData);

            var submittedModel = new SelectOverseasSitesViewModel
            {
                AccreditationId = accreditationId,
                OverseasSites = model.OverseasSites,
                SelectedOverseasSites = new List<string> { "1", "2" }
            };

            // Act
            var result = _controller.CheckOverseasSites(submittedModel, "1");

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var returnedModel = viewResult.Model as SelectOverseasSitesViewModel;
            returnedModel.Should().NotBeNull();
            returnedModel.SelectedOverseasSites.Should().NotContain("1");
            _controller.TempData["RemovedSite"].Should().Be("ABC Exporters Ltd");
            _controller.TempData["SelectOverseasSitesModel"].Should().NotBeNull();
        }

        [TestMethod]
        public void CheckOverseasSites_Post_Continue_RedirectsToCheckAnswers()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var model = new SelectOverseasSitesViewModel
            {
                AccreditationId = accreditationId,
                OverseasSites = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
                {
                    new() { Value = "1", Text = "ABC Exporters Ltd" }
                },
                SelectedOverseasSites = new List<string> { "1" },
                Action = "continue"
            };
            var tempData = new Dictionary<string, object>
            {
                { "SelectOverseasSitesModel", System.Text.Json.JsonSerializer.Serialize(model) }
            };
            SetupTempData(_controller, tempData);

            var submittedModel = new SelectOverseasSitesViewModel
            {
                AccreditationId = accreditationId,
                OverseasSites = model.OverseasSites,
                SelectedOverseasSites = new List<string> { "1" },
                Action = "continue"
            };

            // Act
            var result = _controller.CheckOverseasSites(submittedModel, null);

            // Assert
            result.Should().BeOfType<RedirectToRouteResult>();
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.ExporterAccreditationTaskList);
            redirectResult.RouteValues["accreditationId"].Should().Be(accreditationId);
        }

        [TestMethod]
        public void CheckOverseasSites_Post_Save_RedirectsToApplicationSaved()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var model = new SelectOverseasSitesViewModel
            {
                AccreditationId = accreditationId,
                OverseasSites = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
                {
                    new() { Value = "1", Text = "ABC Exporters Ltd" }
                },
                SelectedOverseasSites = new List<string> { "1" },
                Action = "save"
            };
            var tempData = new Dictionary<string, object>
            {
                { "SelectOverseasSitesModel", System.Text.Json.JsonSerializer.Serialize(model) }
            };
            SetupTempData(_controller, tempData);

            var submittedModel = new SelectOverseasSitesViewModel
            {
                AccreditationId = accreditationId,
                OverseasSites = model.OverseasSites,
                SelectedOverseasSites = new List<string> { "1" },
                Action = "save"
            };

            // Act
            var result = _controller.CheckOverseasSites(submittedModel, null);

            // Assert
            result.Should().BeOfType<RedirectToRouteResult>();
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.ApplicationSaved);
        }

        [TestMethod]
        public void CheckOverseasSites_Post_InvalidAction_ReturnsBadRequest()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var model = new SelectOverseasSitesViewModel
            {
                AccreditationId = accreditationId,
                OverseasSites = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
                {
                    new() { Value = "1", Text = "ABC Exporters Ltd" }
                },
                SelectedOverseasSites = new List<string> { "1" },
                Action = "invalid"
            };
            var tempData = new Dictionary<string, object>
            {
                { "SelectOverseasSitesModel", System.Text.Json.JsonSerializer.Serialize(model) }
            };
            SetupTempData(_controller, tempData);

            var submittedModel = new SelectOverseasSitesViewModel
            {
                AccreditationId = accreditationId,
                OverseasSites = model.OverseasSites,
                SelectedOverseasSites = new List<string> { "1" },
                Action = "invalid"
            };

            // Act
            var result = _controller.CheckOverseasSites(submittedModel, null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.Value.Should().Be("Invalid action supplied.");
        }

        [TestMethod]
        public void CheckOverseasSites_Post_MissingData_ThrowsInvalidOperationException()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            SetupTempData(_controller);

            var submittedModel = new SelectOverseasSitesViewModel
            {
                AccreditationId = accreditationId,
                OverseasSites = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>(),
                SelectedOverseasSites = new List<string> { "1" },
                Action = "continue"
            };

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(() =>
                _controller.CheckOverseasSites(submittedModel, null));
        }
        #endregion

        #region UploadEvidenceOfEquivalentStandards
        [TestMethod]
        public async Task UploadEvidenceOfEquivalentStandards_SiteOutsideEU_OECD_ReturnsViewWithModel()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var accreditation = new AccreditationDto
            {
                ExternalId = accreditationId,
                MaterialName = "Glass"
            };
            var model = new SelectOverseasSitesViewModel
            {
                AccreditationId = accreditationId,
                OverseasSites = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
                {
                    new() { Value = "1", Text = "Hun Manet Recycler Ltd, Tuol Sleng Road, Battambang, Cambodia", Group = new SelectListGroup { Name = "Cambodia" } },
                },
                SelectedOverseasSites = ["1"]
            };
            var tempData = new Dictionary<string, object>
            {
                { "SelectOverseasSitesModel", System.Text.Json.JsonSerializer.Serialize(model) }
            };
            SetupTempData(_controller, tempData);
;
            _mockAccreditationService.Setup(s => s.GetAccreditation(accreditationId)).ReturnsAsync(accreditation);

            // Act
            var result = await _controller.UploadEvidenceOfEquivalentStandards(accreditationId);

            // Assert
            var viewResult = result as ViewResult;
            var viewModel = viewResult.Model as UploadEvidenceOfEquivalentStandardsViewModel;
            viewResult.Should().NotBeNull();
            viewModel.Should().NotBeNull();
            viewModel.MaterialName.Should().Be(accreditation.MaterialName);
            Assert.IsTrue(viewModel.OverseasSites.Count() > 0);
            Assert.IsTrue(viewModel.IsSiteOutsideEU_OECD);
        }

        [TestMethod]
        public async Task UploadEvidenceOfEquivalentStandards_SiteWithinEU_OECD_RedirectsToOptionalUploadOfEvidence()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var model = new SelectOverseasSitesViewModel
            {
                AccreditationId = accreditationId,
                OverseasSites = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
                {
                    new() { Value = "1", Text = "ABC Exporters Ltd, 123 Avenue de la République, Paris 75011, France", Group = new SelectListGroup { Name = "France" } },
                },
                SelectedOverseasSites = ["1"]
            };
            var tempData = new Dictionary<string, object>
            {
                { "SelectOverseasSitesModel", System.Text.Json.JsonSerializer.Serialize(model) }
            };
            SetupTempData(_controller, tempData);

            _mockAccreditationService.Setup(s => s.GetAccreditation(accreditationId)).ReturnsAsync(new AccreditationDto
                                                                                      { ExternalId = accreditationId, MaterialName = "Glass" });

            // Act
            var result = await _controller.UploadEvidenceOfEquivalentStandards(accreditationId);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be(nameof(AccreditationController.OptionalUploadOfEvidenceOfEquivalentStandards));
        }

        [TestMethod]
        public async Task UploadEvidenceOfEquivalentStandards_SiteOutsideEU_OECD_RedirectsToCheckIfYouNeedToUploadEvidence()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var model = new SelectOverseasSitesViewModel
            {
                AccreditationId = accreditationId,
                OverseasSites = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
                {
                    new() { Value = "1", Text = "Hun Manet Recycler Ltd, Tuol Sleng Road, Battambang, Cambodia", Group = new SelectListGroup { Name = "Cambodia" } },
                },
                SelectedOverseasSites = ["1"]
            };
            var tempData = new Dictionary<string, object>
            {
                { "SelectOverseasSitesModel", System.Text.Json.JsonSerializer.Serialize(model) }
            };
            SetupTempData(_controller, tempData);

            _mockAccreditationService.Setup(s => s.GetAccreditation(accreditationId)).ReturnsAsync(new AccreditationDto
                                                                                      { ExternalId = accreditationId, MaterialName = "Steel" });

            // Act
            var result = await _controller.UploadEvidenceOfEquivalentStandards(accreditationId);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be(nameof(AccreditationController.EvidenceOfEquivalentStandardsCheckIfYouNeedToUploadEvidence));
        }
        #endregion

        #region EvidenceOfEquivalentStandardsCheckYourAnswers
        [TestMethod]
        public async Task EvidenceOfEquivalentStandardsCheckYourAnswers_ReturnsViewWithModel()
        {
            // Arrange
            OverseasReprocessingSite overseasSite = new()
            {
                OrganisationName = "Hun Manet Recycler Ltd", AddressLine1 = "Tuol Sleng Road", AddressLine2 = "Battambang", AddressLine3 = "Cambodia"
            };

            // Act
            var result = await _controller.EvidenceOfEquivalentStandardsCheckYourAnswers(
                                overseasSite.OrganisationName, overseasSite.AddressLine1, overseasSite.AddressLine2, overseasSite.AddressLine3);

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult.Model as EvidenceOfEquivalentStandardsCheckYourAnswersViewModel;
            viewResult.Should().NotBeNull();
            model.Should().NotBeNull();
            model.OverseasSite.Should().BeEquivalentTo(overseasSite);
        }
        #endregion

        #region EvidenceOfEquivalentStandardsCheckSiteFulfillsConditions
        [TestMethod]
        public async Task EvidenceOfEquivalentStandardsCheckSiteFulfillsConditions_GetAction_ReturnsViewWithModel()
        {
            // Arrange
            OverseasReprocessingSite overseasSite = new()
            {
                OrganisationName = "Hun Manet Recycler Ltd", AddressLine1 = "Svay Rieng Road", AddressLine2 = "Siem Reap", AddressLine3 = "Cambodia"
            };
            var tempData = new Dictionary<string, object>
            {
                { "AccreditationId", Guid.NewGuid() }
            };
            SetupTempData(_controller, tempData);

            // Act
            var result = await _controller.EvidenceOfEquivalentStandardsCheckSiteFulfillsConditions(
                                overseasSite.OrganisationName, overseasSite.AddressLine1, overseasSite.AddressLine2, overseasSite.AddressLine3);

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult.Model as EvidenceOfEquivalentStandardsCheckSiteFulfillsConditionsViewModel;
            viewResult.Should().NotBeNull();
            model.Should().NotBeNull();
            model.OverseasSite.Should().BeEquivalentTo(overseasSite);
        }

        [TestMethod]
        public async Task EvidenceOfEquivalentStandardsCheckSiteFulfillsConditions_PostActionSave_RedirectsToApplicationSaved()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var accreditation = new AccreditationDto
            {
                ExternalId = accreditationId, MaterialName = "Steel",
                OverseasSiteName = "Hun Manet Recycler Ltd", OverseasSiteCheckedForConditionFulfilment = true
            };
            var model = new EvidenceOfEquivalentStandardsCheckSiteFulfillsConditionsViewModel
            {
                OverseasSite = new OverseasReprocessingSite
                {
                    OrganisationName = "Hun Manet Recycler Ltd", AddressLine1 = "Svay Rieng Road", AddressLine2 = "Siem Reap", AddressLine3 = "Cambodia"
                },
                AccreditationId = accreditationId,
                SelectedOption = FulfilmentsOfWasteProcessingConditions.ConditionsFulfilledEvidenceUploadwanted,
                Action = "save"
            };
            _mockAccreditationService.Setup(s => s.GetAccreditation(model.AccreditationId)).ReturnsAsync(accreditation);
            _mockAccreditationService.Setup(s => s.UpsertAccreditation(It.IsAny<AccreditationRequestDto>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.EvidenceOfEquivalentStandardsCheckSiteFulfillsConditions(model);

            // Assert
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.ApplicationSaved);
        }

        [TestMethod]
        public async Task EvidenceOfEquivalentStandardsCheckSiteFulfillsConditions_PostAction_RedirectsToCheckYourAnswers()
        {
            // Arrange
            var model = new EvidenceOfEquivalentStandardsCheckSiteFulfillsConditionsViewModel
            {
                OverseasSite = new OverseasReprocessingSite
                {
                    OrganisationName = "Hun Manet Recycler Ltd", AddressLine1 = "Svay Rieng Road", AddressLine2 = "Siem Reap", AddressLine3 = "Cambodia"
                },
                SelectedOption = FulfilmentsOfWasteProcessingConditions.ConditionsFulfilledEvidenceUploadUnwanted
            };

            // Act
            var result = await _controller.EvidenceOfEquivalentStandardsCheckSiteFulfillsConditions(model);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be(nameof(AccreditationController.EvidenceOfEquivalentStandardsCheckYourAnswers));
        }
        #endregion

        #region FileUploading

        [TestMethod]
        public async Task FileUploading_When_Submission_DoesNotExist_Returns_ViewResult()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var submissionId = Guid.NewGuid();
            AccreditationSubmission accreditationSubmission = null;

            _mockFileUploadService.Setup(s => s.GetFileUploadSubmissionStatusAsync<AccreditationSubmission>(submissionId)).ReturnsAsync(accreditationSubmission);

            // Act
            var result = await _controller.FileUploading(accreditationId, submissionId);

            // Assert
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.AccreditationSamplingAndInspectionPlan);
            redirectResult.RouteValues["AccreditationId"].Should().Be(accreditationId);

            _mockFileUploadService.Verify(s => s.GetFileUploadSubmissionStatusAsync<AccreditationSubmission>(It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public async Task FileUploading_When_Submission_AccreditationDataComplete_IsTrue_Returns_ViewResult()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var submissionId = Guid.NewGuid();
            var accreditationSubmission = new AccreditationSubmission
            {
                AccreditationDataComplete = true,
            };

            _mockFileUploadService.Setup(s => s.GetFileUploadSubmissionStatusAsync<AccreditationSubmission>(submissionId)).ReturnsAsync(accreditationSubmission);

            // Act
            var result = await _controller.FileUploading(accreditationId, submissionId);

            // Assert
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.AccreditationSamplingAndInspectionPlan);
            redirectResult.RouteValues["AccreditationId"].Should().Be(accreditationId);
            redirectResult.RouteValues["SubmissionId"].Should().Be(submissionId);

            _mockFileUploadService.Verify(s => s.GetFileUploadSubmissionStatusAsync<AccreditationSubmission>(It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public async Task FileUploading_When_Submission_AccreditationDataComplete_IsFalse_Returns_SameView()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var submissionId = Guid.NewGuid();
            var accreditationSubmission = new AccreditationSubmission
            {
                AccreditationDataComplete = false,
            };

            _mockFileUploadService.Setup(s => s.GetFileUploadSubmissionStatusAsync<AccreditationSubmission>(submissionId)).ReturnsAsync(accreditationSubmission);

            // Act
            var result = await _controller.FileUploading(accreditationId, submissionId);

            // Assert
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as FileUploadingViewModel;
            model.Should().NotBeNull();
            model.AccreditationId.Should().Be(accreditationId);
            model.SubmissionId.Should().Be(submissionId);

            _mockFileUploadService.Verify(s => s.GetFileUploadSubmissionStatusAsync<AccreditationSubmission>(It.IsAny<Guid>()), Times.Once);
        }
        #endregion

        #region FileDownload

        [TestMethod]
        public async Task FileDownload_When_File_DoesNotExist_For_Invalid_UploadId_Returns_NotFound()
        {
            // Arrange
            var submissionId = Guid.NewGuid();
            var fileUploadExternalId = Guid.NewGuid();
            var fileId = Guid.NewGuid();

            AccreditationFileUploadDto accreditationFileUploadDto = null;
            _mockAccreditationService.Setup(s => s.GetAccreditationFileUpload(fileUploadExternalId)).ReturnsAsync(accreditationFileUploadDto);

            // Act
            var result = await _controller.FileDownload(fileUploadExternalId, fileId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();

            _mockAccreditationService.Verify(s => s.GetAccreditationFileUpload(fileUploadExternalId), Times.Once);
            _mockFileDownloadService.Verify(s => s.GetFileAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<SubmissionType>(), It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public async Task FileDownload_When_File_Exists_And_Invalid_FileId_Returns_NotFound()
        {
            // Arrange
            var submissionId = Guid.NewGuid();
            var fileUploadExternalId = Guid.NewGuid();
            var fileId = Guid.NewGuid();
            var fileName = "file.csv";
            var fileBytes = Encoding.UTF8.GetBytes("Test file");
            var invalidFileId = Guid.NewGuid();

            var accreditationFileUploadDto = new AccreditationFileUploadDto
            {
                SubmissionId = submissionId,
                ExternalId = fileUploadExternalId,
                FileId = fileId,
                Filename = fileName,
            };

            _mockAccreditationService.Setup(s => s.GetAccreditationFileUpload(fileUploadExternalId)).ReturnsAsync(accreditationFileUploadDto);

            _mockFileDownloadService.Setup(s => s.GetFileAsync(
                accreditationFileUploadDto.FileId.Value,
                accreditationFileUploadDto.Filename,
                SubmissionType.Accreditation,
                submissionId)).ReturnsAsync(fileBytes);

            // Act
            var result = await _controller.FileDownload(fileUploadExternalId, invalidFileId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            _mockAccreditationService.Verify(s => s.GetAccreditationFileUpload(fileUploadExternalId), Times.Once);
            _mockFileDownloadService.Verify(s => s.GetFileAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<SubmissionType>(), It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public async Task FileDownload_When_File_DoesNotExist_Returns_NotFound()
        {
            // Arrange
            var submissionId = Guid.NewGuid();
            var fileUploadExternalId = Guid.NewGuid();
            var fileId = Guid.NewGuid();
            var fileName = "file.csv";
            byte[] fileBytes = null;

            var accreditationFileUploadDto = new AccreditationFileUploadDto
            {
                SubmissionId = submissionId,
                ExternalId = fileUploadExternalId,
                FileId = fileId,
                Filename = fileName,
            };
            _mockAccreditationService.Setup(s => s.GetAccreditationFileUpload(fileUploadExternalId)).ReturnsAsync(accreditationFileUploadDto);

            _mockFileDownloadService.Setup(s => s.GetFileAsync(
                accreditationFileUploadDto.FileId.Value,
                accreditationFileUploadDto.Filename,
                SubmissionType.Accreditation,
                submissionId)).ReturnsAsync(fileBytes);

            // Act
            var result = await _controller.FileDownload(fileUploadExternalId, fileId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();

            _mockAccreditationService.Verify(s => s.GetAccreditationFileUpload(fileUploadExternalId), Times.Once);
            _mockFileDownloadService.Verify(s => s.GetFileAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<SubmissionType>(), It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public async Task FileDownload_Returns_Valid_File()
        {
            // Arrange
            var submissionId = Guid.NewGuid();
            var fileUploadExternalId = Guid.NewGuid();
            var fileId = Guid.NewGuid();
            var fileName = "file.csv";
            var fileBytes = Encoding.UTF8.GetBytes("Test file");

            var accreditationFileUploadDto = new AccreditationFileUploadDto
            {
                SubmissionId = submissionId,
                ExternalId = fileUploadExternalId,
                FileId = fileId,
                Filename = fileName,
            };
            _mockAccreditationService.Setup(s => s.GetAccreditationFileUpload(fileUploadExternalId)).ReturnsAsync(accreditationFileUploadDto);

            _mockFileDownloadService.Setup(s => s.GetFileAsync(
                accreditationFileUploadDto.FileId.Value,
                accreditationFileUploadDto.Filename,
                SubmissionType.Accreditation,
                submissionId)).ReturnsAsync(fileBytes);

            // Act
            var result = await _controller.FileDownload(fileUploadExternalId, fileId);

            // Assert
            var viewResult = result as FileContentResult;
            viewResult.Should().BeOfType<FileContentResult>();
            viewResult.FileDownloadName.Should().Be(fileName);
            viewResult.ContentType.Should().Be("text/csv");

            _mockAccreditationService.Verify(s => s.GetAccreditationFileUpload(fileUploadExternalId), Times.Once);
            _mockFileDownloadService.Verify(s => s.GetFileAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<SubmissionType>(), It.IsAny<Guid>()), Times.Once);
        }
        #endregion

        #region DeleteUploadedFile

        [TestMethod]
        public async Task DeleteUploadedFile_When_File_Exists_And_Invalid_FileId_Cannot_Delete_File()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var submissionId = Guid.NewGuid();
            var fileUploadExternalId = Guid.NewGuid();
            var fileId = Guid.NewGuid();
            var fileName = "file.csv";
            var invalidFileId = Guid.NewGuid();

            var accreditationFileUploadDto = new AccreditationFileUploadDto
            {
                SubmissionId = submissionId,
                ExternalId = fileUploadExternalId,
                FileId = fileId,
                Filename = fileName,
            };

            _mockAccreditationService.Setup(s => s.GetAccreditationFileUpload(fileUploadExternalId)).ReturnsAsync(accreditationFileUploadDto);

            // Act
            var result = await _controller.DeleteUploadedFile(accreditationId, fileUploadExternalId, invalidFileId);

            // Assert
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.AccreditationSamplingAndInspectionPlan);
            redirectResult.RouteValues["AccreditationId"].Should().Be(accreditationId);

            _mockAccreditationService.Verify(s => s.GetAccreditationFileUpload(fileUploadExternalId), Times.Once);
            _mockAccreditationService.Verify(s => s.DeleteAccreditationFileUpload(accreditationId, fileId), Times.Never);
        }

        [TestMethod]
        public async Task DeleteUploadedFile_When_File_Exists_And_Valid_FileId_Deletes_File()
        {
            // Arrange
            SetupTempData(_controller);
            var accreditationId = Guid.NewGuid();
            var submissionId = Guid.NewGuid();
            var fileUploadExternalId = Guid.NewGuid();
            var fileId = Guid.NewGuid();
            var fileName = "file.csv";
            var invalidFileId = Guid.NewGuid();

            var accreditationFileUploadDto = new AccreditationFileUploadDto
            {
                SubmissionId = submissionId,
                ExternalId = fileUploadExternalId,
                FileId = fileId,
                Filename = fileName,
            };

            _mockAccreditationService.Setup(s => s.GetAccreditationFileUpload(fileUploadExternalId)).ReturnsAsync(accreditationFileUploadDto);

            // Act
            var result = await _controller.DeleteUploadedFile(accreditationId, fileUploadExternalId, fileId);

            // Assert
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(AccreditationController.RouteIds.AccreditationSamplingAndInspectionPlan);
            redirectResult.RouteValues["AccreditationId"].Should().Be(accreditationId);

            var notificationBannerModel = _controller.TempData.Get<NotificationBannerModel>(Constants.AccreditationFileDeletedNotification);
            notificationBannerModel.Should().NotBeNull();
            notificationBannerModel.Message.Should().Be($"You've removed {fileName}");

            _mockAccreditationService.Verify(s => s.GetAccreditationFileUpload(fileUploadExternalId), Times.Once);
            _mockAccreditationService.Verify(s => s.DeleteAccreditationFileUpload(accreditationId, fileId), Times.Once);
        }
        #endregion
    }
}