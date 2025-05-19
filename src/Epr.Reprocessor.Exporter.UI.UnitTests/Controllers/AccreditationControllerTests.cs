using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;
using Epr.Reprocessor.Exporter.UI.App.DTOs;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;
using Epr.Reprocessor.Exporter.UI.App.Options;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Controllers;
using Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;
using EPR.Common.Authorization.Models;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;

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

        [TestInitialize]
        public void Setup()
        {
            _controller = new AccreditationController(_mockLocalizer.Object, _mockExternalUrlOptions.Object, _mockValidationService.Object,
                                                      _mockAccountServiceClient.Object, _mockAccreditationService.Object);
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
            _mockAccreditationService.Setup(x => x.GetAccreditation(It.IsAny<Guid>()))
                .ReturnsAsync(new AccreditationDto
                {
                    MaterialName = "Steel",
                });
            var viewModel = new PrnTonnageViewModel { MaterialName = "steel", Action = "continue" };

            // Act
            var result = await _controller.PrnTonnage(viewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var redirectResult = result as RedirectToRouteResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(AccreditationController.RouteIds.SelectPrnTonnage, redirectResult.RouteName);
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
            _mockAccreditationService.Setup(x => x.GetOrganisationUsers(It.IsAny<UserData>()))
                .ReturnsAsync(new List<ManageUserDto> { new ManageUserDto
                {
                    PersonId = Guid.NewGuid(),
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test@user.com"
                } });

            var result = await _controller.SelectAuthority() as ViewResult;

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


            _controller.ModelState.AddModelError("SelectedAuthorities", "Required");

            var model = new SelectAuthorityViewModel() { Action = "continue" };

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
            var model = new SelectAuthorityViewModel() { Action = "continue" };

            _mockValidationService.Setup(v => v.ValidateAsync(model, default))
              .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            // Act
            var result = await _controller.SelectAuthority(model);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual(AccreditationController.RouteIds.CheckAnswersPRNs, (result as RedirectToRouteResult).RouteName);


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
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(AccreditationController.RouteIds.ApplicationSaved, redirectResult.RouteName);

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
            // Act
            var result = _controller.CheckAnswers();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
        }
        #endregion

        #region BusinessPlan
        [TestMethod]
        public async Task BusinessPlan_Get_ReturnsViewResult_WithBusinessPlanViewModel()
        {
            // Act
            var result = await _controller.BusinessPlan();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(BusinessPlanViewModel));
        }
        #endregion

        #region MoreDetailOnBusinessPlan

        [TestMethod]
        public async Task MoreDetailOnBusinessPlan_Get_ReturnsView()
        {
            // Act
            var result = await _controller.MoreDetailOnBusinessPlan();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(MoreDetailOnBusinessPlanViewModel));
            var model = viewResult.ViewData.Model as MoreDetailOnBusinessPlanViewModel;
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public async Task MoreDetailOnBusinessPlan_Post_InvalidModelState_ReturnsViewResult_WithSameModel()
        {
            // Arrange
            _controller.ModelState.AddModelError("Infrastructure", "Infrastructure must be 300 characters or less");
            var viewModel = new MoreDetailOnBusinessPlanViewModel() { Action = "continue" };

            // Act
            var result = await _controller.MoreDetailOnBusinessPlan(viewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(MoreDetailOnBusinessPlanViewModel));
            var model = viewResult.ViewData.Model as MoreDetailOnBusinessPlanViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(viewModel, model);
        }

        [TestMethod]
        public async Task MoreDetailOnBusinessPlan_Post_ActionIsContinue_ReturnsRedirectToCheckAnswers()
        {
            // Arrange
            var viewModel = new MoreDetailOnBusinessPlanViewModel() { Action = "continue" };

            // Act
            var result = await _controller.MoreDetailOnBusinessPlan(viewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var redirectResult = result as RedirectToRouteResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(AccreditationController.RouteIds.MoreDetailOnBusinessPlanPRNs, redirectResult.RouteName);
        }

        [TestMethod]
        public async Task MoreDetailOnBusinessPlan_Post_ActionIsSave_ReturnsRedirectToApplicationSaved()
        {
            // Arrange
            var viewModel = new MoreDetailOnBusinessPlanViewModel() { Action = "save" };

            // Act
            var result = await _controller.MoreDetailOnBusinessPlan(viewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var redirectResult = result as RedirectToRouteResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(AccreditationController.RouteIds.ApplicationSaved, redirectResult.RouteName);
        }

        [TestMethod]
        public async Task MoreDetailOnBusinessPlan_Post_ActionIsUnknown_ReturnsBadRequest()
        {
            // Arrange
            var viewModel = new MoreDetailOnBusinessPlanViewModel() { Action = "unknown" };

            // Act
            var result = await _controller.MoreDetailOnBusinessPlan(viewModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual("Invalid action supplied.", (result as BadRequestObjectResult).Value);
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
