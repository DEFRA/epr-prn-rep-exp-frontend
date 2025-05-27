using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Epr.Reprocessor.Exporter.UI.Controllers;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using System.Diagnostics;
using System.Security.Claims;
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
        private UserData _userData = new()
        {
            FirstName = "Test",
            LastName = "User",
            Organisations = [
                    new Organisation()
        {
            OrganisationNumber = "Test123",
                    Name = "TestOrgName",
                }]
        };

        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _mockOptions = new Mock<IOptions<HomeViewModel>>();

            var homeSettings = new HomeViewModel
            {
                ApplyForRegistration = "/apply-for-registration",
                ViewApplications = "/view-applications"
            };

            _mockOptions.Setup(x => x.Value).Returns(homeSettings);

            _controller = new HomeController(_mockLogger.Object, _mockOptions.Object);


            var jsonUserData = JsonSerializer.Serialize(_userData);
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
        }

        [TestMethod]
        public void Index_redirects_to_ManageOrganisation()
        {
            var result = _controller.Index();

            var redirect = result.Should().BeOfType<RedirectToActionResult>().Which;

            redirect.ActionName.Should().Be(nameof(HomeController.ManageOrganisation));
        }

        [TestMethod]
        public void ManageOrganisation_ReturnsViewResult()
        {
            // Act
            var result = _controller.ManageOrganisation();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsInstanceOfType(viewResult.Model, typeof(HomeViewModel));

            var model = viewResult.Model as HomeViewModel;
            model.Should().BeEquivalentTo(new HomeViewModel()
            {
                FirstName = _userData.FirstName,
                LastName = _userData.LastName,
                ApplyForRegistration = "/apply-for-registration",
                ViewApplications = "/view-applications",
                OrganisationName = _userData.Organisations[0].Name,
                OrganisationNumber = _userData.Organisations[0].OrganisationNumber
            });
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
