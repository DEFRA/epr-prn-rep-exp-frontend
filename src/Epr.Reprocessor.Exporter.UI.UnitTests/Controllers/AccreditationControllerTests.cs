using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.Options;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Controllers;
using Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;
using EPR.Common.Authorization.Models;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using CheckAnswersViewModel = Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation.CheckAnswersViewModel;
using Moq;
using System.ComponentModel.DataAnnotations;
using static Epr.Reprocessor.Exporter.UI.Controllers.AccreditationController;
using Microsoft.AspNetCore.Routing.Patterns;

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
        private Mock<IAccreditationService> _mockAccreditationService = new();
        private Mock<ClaimsPrincipal> _claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        private Mock<IValidationService> _mockValidationService = new();
        private Mock<IUrlHelper> _mockUrlHelperMock = new();

        [TestInitialize]
        public void Setup()
        {
            _controller = new AccreditationController(
                _mockLocalizer.Object,
                _mockExternalUrlOptions.Object,
                _mockValidationService.Object,
                _mockAccountServiceClient.Object,
                _mockAccreditationService.Object);

            _controller.Url = _mockUrlHelperMock.Object;
            _userData = GetUserData("Producer");
            SetupUserData(_userData);
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
            Assert.IsTrue(model.ApprovedPersons.Count() > 0);
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
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(PrnTonnageViewModel));
            var model = viewResult.ViewData.Model as PrnTonnageViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual("steel", model.MaterialName);
        }

        [TestMethod]
        public async Task PrnTonnage_Post_InvalidViewModel_ReturnsSameView()
        {
            // Arrange
            _controller.ModelState.AddModelError("PrnTonnage", "Required");
            var viewModel = new PrnTonnageViewModel { MaterialName = "steel", Action = "continue" };

            // Act
            var result = await _controller.PrnTonnage(viewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(PrnTonnageViewModel));
            var model = viewResult.ViewData.Model as PrnTonnageViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(viewModel, model);
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
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
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
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var redirectResult = result as RedirectToRouteResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(AccreditationController.RouteIds.ApplicationSaved, redirectResult.RouteName);
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
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual("Invalid action supplied.", (result as BadRequestObjectResult).Value);
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

            var result = await _controller.SelectAuthority(accreditationId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as SelectAuthorityViewModel;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Authorities.Count > 0);
        }

        [TestMethod]
        public async Task SelectAuthority_Post_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            _controller.ModelState.AddModelError("SelectedAuthorities", "Required");

            var model = new SelectAuthorityViewModel() { AccreditationId = accreditationId, Action = "continue" };

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
            var model = new SelectAuthorityViewModel() { AccreditationId = accreditationId, Action = "continue" };

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
            var model = new SelectAuthorityViewModel() { Action = "save" };

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

            var backlink = _controller.ViewBag.BackLinkToDisplay as string;
            backlink.Should().Be(backUrl);
        }

        [TestMethod]
        public async Task CheckAnswers_Post_ActionIsContinue_ReturnsRedirectToSelectAuthority()
        {
            // Arrange
            var viewModel = new CheckAnswersViewModel { AccreditationId = Guid.NewGuid(), PrnTonnage = 500, AuthorisedUsers = "First Last, Test User", Action = "continue" };

            // Act
            var result = await _controller.CheckAnswers(viewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var redirectResult = result as RedirectToRouteResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(AccreditationController.RouteIds.AccreditationTaskList, redirectResult.RouteName);
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
                InfrastructurePercentage = 20,
                PackagingWastePercentage = 20,
                BusinessCollectionsPercentage = 20,
                CommunicationsPercentage = 10,
                NewMarketsPercentage = 15,
                NewUsesPercentage = 15,
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
                InfrastructurePercentage = 40,
                PackagingWastePercentage = 20,
                BusinessCollectionsPercentage = 10,
                CommunicationsPercentage = 10,
                NewMarketsPercentage = 10,
                NewUsesPercentage = 10,
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
                InfrastructurePercentage = 50,
                PackagingWastePercentage = 50,
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
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as MoreDetailOnBusinessPlanViewModel;
            Assert.IsNotNull(model);

            Assert.AreEqual(accreditation.ExternalId, model.ExternalId);
            Assert.AreEqual("PERN", model.Subject);
            Assert.IsTrue(model.ShowInfrastructure);
            Assert.AreEqual("Infra note", model.Infrastructure);
            Assert.IsTrue(model.ShowBusinessCollections);
            Assert.AreEqual("Biz note", model.BusinessCollections);
            Assert.IsTrue(model.ShowNewMarkets);
            Assert.AreEqual("Market note", model.NewMarkets);
            Assert.IsFalse(model.ShowCommunications);
        }

        [TestMethod]
        public async Task MoreDetailOnBusinessPlan_Post_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var viewModel = new MoreDetailOnBusinessPlanViewModel
            {
                ExternalId = Guid.NewGuid(),
                Action = "continue"
            };

            _controller.ModelState.AddModelError("Infrastructure", "Too long");

            // Act
            var result = await _controller.MoreDetailOnBusinessPlan(viewModel);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as MoreDetailOnBusinessPlanViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(viewModel, model);
            Assert.AreEqual("PERN", model.Subject);
        }

        [TestMethod]
        public async Task MoreDetailOnBusinessPlan_Post_ActionIsContinue_RedirectsToCheckAnswers()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var viewModel = new MoreDetailOnBusinessPlanViewModel
            {
                ExternalId = accreditationId,
                Action = "continue"
            };

            _mockAccreditationService
                .Setup(x => x.GetAccreditation(accreditationId))
                .ReturnsAsync(new AccreditationDto());

            // Act
            var result = await _controller.MoreDetailOnBusinessPlan(viewModel);

            // Assert
            var redirect = result as RedirectToRouteResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual(AccreditationController.RouteIds.CheckAnswersPERNs, redirect.RouteName);
        }

        [TestMethod]
        public async Task MoreDetailOnBusinessPlan_Post_ActionIsSave_RedirectsToApplicationSaved()
        {
            // Arrange
            var accreditationId = Guid.NewGuid();
            var viewModel = new MoreDetailOnBusinessPlanViewModel
            {
                ExternalId = accreditationId,
                Action = "save"
            };

            _mockAccreditationService
                .Setup(x => x.GetAccreditation(accreditationId))
                .ReturnsAsync(new AccreditationDto());

            // Act
            var result = await _controller.MoreDetailOnBusinessPlan(viewModel);

            // Assert
            var redirect = result as RedirectToRouteResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual(AccreditationController.RouteIds.ApplicationSaved, redirect.RouteName);
        }

        [TestMethod]
        public async Task MoreDetailOnBusinessPlan_Post_InvalidAction_ReturnsBadRequest()
        {
            // Arrange
            var viewModel = new MoreDetailOnBusinessPlanViewModel
            {
                ExternalId = Guid.NewGuid(),
                Action = "invalid"
            };

            _mockAccreditationService
                .Setup(x => x.GetAccreditation(viewModel.ExternalId))
                .ReturnsAsync(new AccreditationDto());

            // Act
            var result = await _controller.MoreDetailOnBusinessPlan(viewModel);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual("Invalid action supplied.", badRequest.Value);
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
        public async Task TaskList_Get_ReturnsViewResult()
        {
            // Act
            var result = await _controller.TaskList();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
        }
        #endregion

        #region ReviewBusinessPlan

        [TestMethod]
        public void ReviewBusinessPlan_ReturnsViewResult()
        {
            // Act
            var result = _controller.ReviewBusinessPlan();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult), "Expected a ViewResult to be returned.");
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult, "Expected the ViewResult to not be null.");
        }



        #endregion

        #region SamplingAndInspectionPlan

        [TestMethod]
        public async Task SamplingAndInspectionPlan_Get_ReturnsView()
        {
            // Act
            var result = await _controller.SamplingAndInspectionPlan();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(SamplingAndInspectionPlanViewModel));
        }

        #endregion
    }
}
