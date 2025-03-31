using Epr.Reprocessor.Exporter.UI.Constants;
using Epr.Reprocessor.Exporter.UI.Controllers;
using Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Tests.Controllers
{
    [TestClass]
    public class AccreditationControllerTests
    {
        [TestMethod]
        public async Task PrnTonnage_Get_ReturnsViewResult_WithPrnTonnageViewModel()
        {
            // Arrange
            var controller = new AccreditationController();

            // Act
            var result = await controller.PrnTonnage();

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
        public async Task PrnTonnage_Post_InvalidModelState_ReturnsViewResult_WithSameModel()
        {
            // Arrange
            var controller = new AccreditationController();
            controller.ModelState.AddModelError("PrnTonnage", "Required");
            var viewModel = new PrnTonnageViewModel { MaterialName = "steel" };

            // Act
            var result = await controller.PrnTonnage(viewModel, "continue");

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
        public async Task PrnTonnage_Post_ValidModelState_ContinueAction_RedirectsToSelectAuthority()
        {
            // Arrange
            var controller = new AccreditationController();
            var viewModel = new PrnTonnageViewModel { MaterialName = "steel" };

            // Act
            var result = await controller.PrnTonnage(viewModel, "continue");

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var redirectToRouteResult = result as RedirectToRouteResult;
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual(PagePath.SelectAuthority, redirectToRouteResult.RouteName);
        }

        [TestMethod]
        public async Task PrnTonnage_Post_ValidModelState_SaveAction_RedirectsToApplicationSaved()
        {
            // Arrange
            var controller = new AccreditationController();
            var viewModel = new PrnTonnageViewModel { MaterialName = "steel" };

            // Act
            var result = await controller.PrnTonnage(viewModel, "save");

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var redirectToRouteResult = result as RedirectToRouteResult;
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual(PagePath.ApplicationSaved, redirectToRouteResult.RouteName);
        }

        [TestMethod]
        public async Task PrnTonnage_Post_ValidModelState_UnknownAction_RedirectsToIndex()
        {
            // Arrange
            var controller = new AccreditationController();
            var viewModel = new PrnTonnageViewModel { MaterialName = "steel" };

            // Act
            var result = await controller.PrnTonnage(viewModel, "unknown");

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(PrnTonnageViewModel));
            var model = viewResult.ViewData.Model as PrnTonnageViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual("steel", model.MaterialName);
        }
    }
}
