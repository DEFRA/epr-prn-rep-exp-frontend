using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.Controllers;
using Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;
using Epr.Reprocessor.Exporter.UI.ViewModels.Reprocessor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Moq;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers
{
    [TestClass]
    public class AccreditationControllerTests
    {
        private AccreditationController _controller;
        private Mock<IStringLocalizer<SharedResources>> _mockLocalizer = new();

        [TestInitialize]
        public void Setup()
        {
            _controller = new AccreditationController(_mockLocalizer.Object);
        }

        #region ApplicationSaved
        [TestMethod]
        public async Task ApplicationSaved_ReturnsExpectedViewResult()
        {
            // Act
            var result =  _controller.ApplicationSaved();

            // Assert
            Assert.AreSame(typeof(ViewResult), result.GetType(), "Result should be of type ViewResult");
            
        }
        #endregion

        #region NotAnApprovedPerson

        [TestMethod]
        public async Task NotAnApprovedPerson_Get_ReturnsView()
        {
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

        #region SelectMaterial

        [TestMethod]
        public async Task SelectMaterial_Get_ReturnsView()
        {
            // Act
            var result = await _controller.SelectMaterial();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(SelectMaterialViewModel));
            var model = viewResult.ViewData.Model as SelectMaterialViewModel;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Materials.Any());
        }

        [TestMethod]
        public async Task SelectMaterial_PostWithInvalidViewModel_ReturnsSameView()
        {
            // Arrange
            _controller.ModelState.AddModelError("SelectedMaterial", "Required");
            var viewModel = new SelectMaterialViewModel();

            // Act
            var result = await _controller.SelectMaterial(viewModel, "continue");

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(SelectMaterialViewModel));
            var model = viewResult.ViewData.Model as SelectMaterialViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(viewModel, model);
        }

        [TestMethod]
        public async Task SelectMaterial_PostWithValidViewModel_ReturnsRedirectToTaskList()
        {
            // Arrange
            var viewModel = new SelectMaterialViewModel();

            // Act
            var result = await _controller.SelectMaterial(viewModel, "continue");

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(PagePaths.SelectMaterial, redirectResult.Url);
        }

        #endregion

        #region PrnTonnage

        [TestMethod]
        public async Task PrnTonnage_Get_ReturnsView()
        {
            // Act
            var result = await _controller.PrnTonnage();

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
            var viewModel = new PrnTonnageViewModel { MaterialName = "steel" };

            // Act
            var result = await _controller.PrnTonnage(viewModel, "continue");

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
            var viewModel = new PrnTonnageViewModel { MaterialName = "steel" };

            // Act
            var result = await _controller.PrnTonnage(viewModel, "continue");

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(PagePaths.SelectPrnTonnage, redirectResult.Url);
        }

        [TestMethod]
        public async Task PrnTonnage_Post_ActionIsSave_ReturnsRedirectToApplicationSaved()
        {
            // Arrange
            var viewModel = new PrnTonnageViewModel { MaterialName = "steel" };

            // Act
            var result = await _controller.PrnTonnage(viewModel, "save");

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(PagePaths.ApplicationSaved, redirectResult.Url);
        }

        [TestMethod]
        public async Task PrnTonnage_Post_ActionIsUnknown_ReturnsBadRequest()
        {
            // Arrange
            var viewModel = new PrnTonnageViewModel { MaterialName = "steel" };

            // Act
            var result = await _controller.PrnTonnage(viewModel, "unknown");

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

            // Act
            var result = await _controller.SelectAuthority(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(model, result.Model);
        }

        [TestMethod]
        public async Task SelectAuthority_Post_ValidModelState_ContinueAction_RedirectsToCheckAnswers()
        {

            // Arrange
            var model = new SelectAuthorityViewModel() { Action = "continue" };

            // Act
            var result = await _controller.SelectAuthority(model);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual("Invalid action supplied: continue.", (result as BadRequestObjectResult).Value);

        }

        [TestMethod]
        public async Task SelectAuthority_Post_ValidModelState_SaveAction_RedirectsToApplicationSaved()
        {
            // Arrange
            var model = new SelectAuthorityViewModel() { Action = "save" };

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

            // Simulate model validation
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();
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
            var viewModel = new MoreDetailOnBusinessPlanViewModel();

            // Act
            var result = await _controller.MoreDetailOnBusinessPlan(viewModel, "continue");

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
            var viewModel = new MoreDetailOnBusinessPlanViewModel();

            // Act
            var result = await _controller.MoreDetailOnBusinessPlan(viewModel, "continue");

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(PagePaths.MoreDetailOnBusinessPlan, redirectResult.Url);
        }

        [TestMethod]
        public async Task MoreDetailOnBusinessPlan_Post_ActionIsSave_ReturnsRedirectToApplicationSaved()
        {
            // Arrange
            var viewModel = new MoreDetailOnBusinessPlanViewModel();

            // Act
            var result = await _controller.MoreDetailOnBusinessPlan(viewModel, "save");

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            var redirectResult = result as RedirectResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(PagePaths.ApplicationSaved, redirectResult.Url);
        }

        [TestMethod]
        public async Task MoreDetailOnBusinessPlan_Post_ActionIsUnknown_ReturnsBadRequest()
        {
            // Arrange
            var viewModel = new MoreDetailOnBusinessPlanViewModel();

            // Act
            var result = await _controller.MoreDetailOnBusinessPlan(viewModel, "unknown");

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


    }
}
