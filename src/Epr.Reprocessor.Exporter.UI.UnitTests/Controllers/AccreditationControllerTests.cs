using System;
using System.Collections.Generic;
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

        [TestMethod]
        public async Task ApplicationSaved_ReturnsExpectedViewResult()
        {
            // Act
            var result =  _controller.ApplicationSaved();

            // Assert
            Assert.AreSame(typeof(ViewResult), result.GetType(), "Result should be of type ViewResult");
            
        }

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
    }
}
