﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Epr.Reprocessor.Exporter.UI.Controllers;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using System.Diagnostics;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Organisation = EPR.Common.Authorization.Models.Organisation;

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
                        Id = Guid.NewGuid(),
                        OrganisationNumber = "Test123",
                        Name = "TestOrgName",
                    }
            ]
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
        public void Index_redirects_to_ManageOrganisationIf_Organisation_Exists()
        {
            var result = _controller.Index();

            var redirect = result.Should().BeOfType<RedirectToActionResult>().Which;

            redirect.ActionName.Should().Be(nameof(HomeController.ManageOrganisation));
        }

        [TestMethod]
        public void Index_redirects_to_AddOrganisationIf_UserExistsButNo_Organisation()
        {
            var jsonUserData = JsonSerializer.Serialize(new UserData() { FirstName = "UserWOOrg"});
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

            var result = _controller.Index();

            var redirect = result.Should().BeOfType<RedirectToActionResult>().Which;

            redirect.ActionName.Should().Be(nameof(HomeController.AddOrganisation));
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
        
        [TestMethod]
        public void Index_redirects_to_SelectOrganisationIf_Multiple_Organisations_Exist()
        {
            _userData.Organisations.Add(new Organisation()
            {
                Id = Guid.NewGuid(),
                OrganisationNumber = "Test456",
                Name = "AnotherOrg"
            });

            var jsonUserData = JsonSerializer.Serialize(_userData);
            var claims = new[]
            {
                new Claim(ClaimTypes.UserData, jsonUserData)
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var result = _controller.Index();

            var redirect = result.Should().BeOfType<RedirectToActionResult>().Which;
            redirect.ActionName.Should().Be(nameof(HomeController.SelectOrganisation));
        }
        
        [TestMethod]
        public void SelectOrganisation_ReturnsViewResultWithCorrectModel()
        {
            var result = _controller.SelectOrganisation();

            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeOfType<SelectOrganisationViewModel>();

            var model = viewResult.Model as SelectOrganisationViewModel;
            model.FirstName.Should().Be(_userData.FirstName);
            model.LastName.Should().Be(_userData.LastName);
            model.Organisations.Should().HaveCount(_userData.Organisations.Count);
            model.Organisations[0].OrganisationName.Should().Be(_userData.Organisations[0].Name);
            model.Organisations[0].OrganisationNumber.Should().Be(_userData.Organisations[0].OrganisationNumber);
        }
    }
}
