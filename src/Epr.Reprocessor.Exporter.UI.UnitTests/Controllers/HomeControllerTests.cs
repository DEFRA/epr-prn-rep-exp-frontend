using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Epr.Reprocessor.Exporter.UI.Controllers;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using EPR.Common.Authorization.Models;
using System.Text.Json;

namespace Epr.Reprocessor.Exporter.UI.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        private Mock<ILogger<HomeController>> _mockLogger;
        private Mock<IOptions<HomeViewModel>> _mockOptions;
        private HomeController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _mockOptions = new Mock<IOptions<HomeViewModel>>();

            var homeSettings = new HomeViewModel
            {
                AddOrganisation = "/add-organisation",
                ViewOrganisations = "/view-organisations",
                ApplyReprocessor = "/apply-for-reprocessor-registration",
                ApplyExporter = "/apply-for-exporter-registration",
                ViewApplications = "/view-applications"
            };

            _mockOptions.Setup(x => x.Value).Returns(homeSettings);

            _controller = new HomeController(_mockLogger.Object, _mockOptions.Object);
        }

        [TestMethod]
        public void Index_ReturnsViewResult()
        {
            // Arrange
            var userData = new UserData
            {
                FirstName = "Test",
                LastName = "User",
                Organisations = new List<Organisation>()
            };

            var jsonUserData = JsonSerializer.Serialize(userData);

            var claims = new[]
                    {
                new Claim(ClaimTypes.UserData, jsonUserData)
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Assign the fake user to controller context
            var httpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsInstanceOfType(viewResult.Model, typeof(HomeViewModel));

            var model = viewResult.Model as HomeViewModel;
            Assert.AreEqual("Test", model.FirstName);
            Assert.AreEqual("User", model.LastName);
            Assert.AreEqual("/add-organisation", model.AddOrganisation);
            Assert.AreEqual("/view-organisations", model.ViewOrganisations);
            Assert.AreEqual("/apply-for-reprocessor-registration", model.ApplyReprocessor);
            Assert.AreEqual("/apply-for-exporter-registration", model.ApplyExporter);
            Assert.AreEqual("/view-applications", model.ViewApplications);
        }

        [TestMethod]
        public void Privacy_ReturnsViewResult()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Error_ReturnsViewResultWithErrorViewModel()
        {
            // Arrange
            var activity = new Activity("test");
            activity.Start();
            Activity.Current = activity;

            // Act
            var result = _controller.Error() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ErrorViewModel));
            var model = result.Model as ErrorViewModel;
            Assert.AreEqual(activity.Id, model.RequestId);
        }
    }
}
