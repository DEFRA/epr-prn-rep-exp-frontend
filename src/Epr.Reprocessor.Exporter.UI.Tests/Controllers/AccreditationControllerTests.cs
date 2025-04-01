using Epr.Reprocessor.Exporter.UI.Constants;
using Epr.Reprocessor.Exporter.UI.Controllers;
using Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace Epr.Reprocessor.Exporter.UI.Tests.Controllers
{

    [TestClass]
    public class AccreditationControllerTests
    {
        private readonly Mock<ITempDataDictionary> _tempDataMock;
        private readonly AccreditationController _controller;

        public AccreditationControllerTests()
        {
            _tempDataMock = new Mock<ITempDataDictionary>();
            _controller = new AccreditationController
            {
                TempData = _tempDataMock.Object
            };
        }

        [TestMethod]
        public async Task PrnTonnage_Get_ReturnsViewResult_WithPrnTonnageViewModel()
        {
            // Arrange
            var controller = new AccreditationController();

            // Act
            var result = controller.PrnTonnage();

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
            var result = controller.PrnTonnage(viewModel, "continue");

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
            var result = controller.PrnTonnage(viewModel, "continue");

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
            var result = controller.PrnTonnage(viewModel, "save");

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
            var result = controller.PrnTonnage(viewModel, "unknown");

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
        public async Task SelectAuthority_Get_ReturnsViewResult_WithSelectAuthorityModel()
        {
            // Act
            var result = await _controller.SelectAuthority();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(SelectAuthorityModel));
            var model = viewResult.ViewData.Model as SelectAuthorityModel;
            Assert.IsNotNull(model);
            Assert.IsNotNull(model.Authorities);
        }

        [TestMethod]
        public async Task SelectAuthority_Post_InvalidModel_ReturnsViewResult_WithModel()
        {
            // Arrange
            var model = new SelectAuthorityModel();
            _controller.ModelState.AddModelError("error", "some error");

            // Act
            var result = await _controller.SelectAuthority(model, "continue");

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(SelectAuthorityModel));
            var returnedModel = viewResult.ViewData.Model as SelectAuthorityModel;
            Assert.AreEqual(model, returnedModel);
        }

        [TestMethod]
        public async Task SelectAuthority_Post_ValidModel_ContinueAction_RedirectsToCheckAnswers()
        {
            // Arrange
            var model = new SelectAuthorityModel();

            // Act
            var result = await _controller.SelectAuthority(model, "continue");

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var redirectToRouteResult = result as RedirectToRouteResult;
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual(PagePath.CheckAnswers, redirectToRouteResult.RouteName);
        }

        [TestMethod]
        public async Task SelectAuthority_Post_ValidModel_SaveAction_RedirectsToApplicationSaved()
        {
            // Arrange
            var model = new SelectAuthorityModel();

            // Act
            var result = await _controller.SelectAuthority(model, "save");

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var redirectToRouteResult = result as RedirectToRouteResult;
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual(PagePath.ApplicationSaved, redirectToRouteResult.RouteName);
        }
    }
}
