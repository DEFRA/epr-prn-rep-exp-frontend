using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.Constants;
using Epr.Reprocessor.Exporter.UI.Controllers;
using Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace Epr.Reprocessor.Exporter.UI.Tests.Controllers
{

    [TestClass]
    public class AccreditationControllerTests
    {
        [TestMethod]
        public async Task PrnTonnage_Get_ReturnsViewWithViewModel()
        {
            // Arrange
            var controller = new AccreditationController();

            // Act
            var result = await controller.PrnTonnage() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var viewModel = result.Model as PrnTonnageViewModel;
            Assert.IsNotNull(viewModel);
            Assert.AreEqual("steel", viewModel.MaterialName);
        }

        [TestMethod]
        public async Task PrnTonnage_Post_InvalidModelState_ReturnsViewWithViewModel()
        {
            // Arrange
            var controller = new AccreditationController();
            controller.ModelState.AddModelError("PrnTonnage", "Required");

            var viewModel = new PrnTonnageViewModel
            {
                MaterialName = "steel"
            };

            // Act
            var result = await controller.PrnTonnage(viewModel, "continue") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(viewModel, result.Model);
        }

        [TestMethod]
        public async Task PrnTonnage_Post_ValidModelState_ContinueAction_RedirectsToSelectAuthority()
        {
            // Arrange
            var controller = new AccreditationController();

            var viewModel = new PrnTonnageViewModel
            {
                MaterialName = "steel"
            };

            // Act
            var result = await controller.PrnTonnage(viewModel, "continue") as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(PagePath.SelectAuthority, result.RouteName);
        }

        [TestMethod]
        public async Task PrnTonnage_Post_ValidModelState_SaveAction_RedirectsToApplicationSaved()
        {
            // Arrange
            var controller = new AccreditationController();

            var viewModel = new PrnTonnageViewModel
            {
                MaterialName = "steel"
            };

            // Act
            var result = await controller.PrnTonnage(viewModel, "save") as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(PagePath.ApplicationSaved, result.RouteName);
        }

        [TestMethod]
        public async Task SelectAuthority_Get_ReturnsViewWithModel()
        {
            // Arrange
            var controller = new AccreditationController();

            // Act
            var result = await controller.SelectAuthority() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as SelectAuthorityModel;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Authorities.Count > 0);
        }

        [TestMethod]
        public async Task SelectAuthority_Post_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var controller = new AccreditationController();
            controller.ModelState.AddModelError("SelectedAuthorities", "Required");

            var model = new SelectAuthorityModel();

            // Act
            var result = await controller.SelectAuthority(model, "continue") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(model, result.Model);
        }

        [TestMethod]
        public async Task SelectAuthority_Post_ValidModelState_ContinueAction_RedirectsToCheckAnswers()
        {
            // Arrange
            var controller = new AccreditationController();

            var model = new SelectAuthorityModel();

            // Act
            var result = await controller.SelectAuthority(model, "continue");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual("Invalid action supplied: continue.", (result as BadRequestObjectResult).Value);

        }

        [TestMethod]
        public async Task SelectAuthority_Post_ValidModelState_SaveAction_RedirectsToApplicationSaved()
        {
            // Arrange
            var controller = new AccreditationController();

            var model = new SelectAuthorityModel();

            // Act
            var result = await controller.SelectAuthority(model, "save");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual("Invalid action supplied: save.", (result as BadRequestObjectResult).Value);

        }

        [TestMethod]
        public async Task SelectAuthority_Post_InvalidSelectedAuthoritiesCount_ReturnsView()
        {
            // Arrange
            var controller = new AccreditationController();

            var model = new SelectAuthorityModel
            {
                SelectedAuthorities = new List<string>(), // No authorities selected
            };

            // Simulate model validation
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(model, validationContext, validationResults, true))
            {
                foreach (var validationResult in validationResults)
                {
                    controller.ModelState.AddModelError(string.Empty, validationResult.ErrorMessage);
                }
            }

            // Act
            var result = await controller.SelectAuthority(model, "continue") as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Expected a ViewResult to be returned.");
            Assert.AreEqual(model, result.Model, "Expected the same model to be returned.");
            Assert.AreEqual(0, (result.Model as SelectAuthorityModel).SelectedAuthoritiesCount);
            Assert.IsFalse(controller.ModelState.IsValid, "Expected ModelState to be invalid.");
        }

        [TestMethod]
        public async Task CheckAnswers_Get_ReturnsView()
        {
            // Arrange
            var controller = new AccreditationController();

            // Act
            var result = await controller.CheckAnswers() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
