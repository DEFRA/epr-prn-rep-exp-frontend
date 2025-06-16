using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Epr.Reprocessor.Exporter.UI.Controllers;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.App.Enums.Accreditation;
using System.Diagnostics;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Organisation = EPR.Common.Authorization.Models.Organisation;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

namespace Epr.Reprocessor.Exporter.UI.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        private Mock<ILogger<HomeController>> _mockLogger;
        private Mock<IOptions<LinksConfig>> _mockLinksConfig;
        private Mock<IRegistrationService> _mockRegistrationService;
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
            _mockLinksConfig = new Mock<IOptions<LinksConfig>>();
            _mockRegistrationService = new Mock<IRegistrationService>();

            var linksConfig = new LinksConfig
            {
                ApplyForRegistration = "/apply-for-registration",
                ViewApplications = "/view-applications"
            };

            _mockLinksConfig.Setup(x => x.Value).Returns(linksConfig);

            _controller = new HomeController(_mockLogger.Object, _mockLinksConfig.Object, _mockRegistrationService.Object);

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
            var jsonUserData = JsonSerializer.Serialize(new UserData() { FirstName = "UserWOOrg" });
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
            redirect.ActionName.Should().Be(nameof(HomeController.AddOrganisation));
        }

        [TestMethod]
        public async Task ManageOrganisation_ReturnsViewResultWithCorrectModel()
        {
            // Arrange
            var registrationData = new List<RegistrationDto>
            {
                new(){
                RegistrationId = Guid.NewGuid(),
                RegistrationStatus = (App.Enums.Registration.RegistrationStatus)RegistrationStatus.Granted,
                AccreditationStatus = (App.Enums.Accreditation.AccreditationStatus)Epr.Reprocessor.Exporter.UI.Enums.AccreditationStatus.NotAccredited,
                ApplicationType = "Test Type",
                Year = 2024,
                ApplicationTypeId = 1,
                MaterialId = 1,
                Material = "Test Material",
                ReprocessingSiteId = 1,
                RegistrationMaterialId = 1,
                ReprocessingSiteAddress = new AddressDto() {
                    Id = 1,
                    AddressLine1 = "Test Address",
                    AddressLine2 = "Test street",
                    TownCity = "Test City"
                }
               }
            };

            _mockRegistrationService
                .Setup(x => x.GetRegistrationAndAccreditationAsync(It.IsAny<Guid?>()))
                .ReturnsAsync(registrationData);

            // Act
            var result = await _controller.ManageOrganisation();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeOfType<HomeViewModel>();

            var model = viewResult.Model as HomeViewModel;
            model.FirstName.Should().Be(_userData.FirstName);
            model.LastName.Should().Be(_userData.LastName);
            model.OrganisationName.Should().Be(_userData.Organisations[0].Name);
            model.OrganisationNumber.Should().Be(_userData.Organisations[0].OrganisationNumber);
            model.ApplyForRegistration.Should().Be("/apply-for-registration");
            model.ViewApplications.Should().Be("/view-applications");
            model.RegistrationData.Should().HaveCount(1);
            model.RegistrationData[0].Material.Should().Be("Test Material<br />Test Type");
            model.RegistrationData[0].SiteAddress.Should().Be("Test Address, Test City");
        }

        [TestMethod]
        public void AddOrganisation_ReturnsOkResult()
        {
            var result = _controller.AddOrganisation();

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be("This is place holder for add organisation logic which need new view saying you don't have any org add new org and still on discussion");
        }

        [TestMethod]
        public void Privacy_ReturnsViewResult()
        {
            var result = _controller.Privacy();
            result.Should().BeOfType<ViewResult>();
        }

        [TestMethod]
        public void Error_ReturnsViewResultWithErrorViewModel()
        {
            // Arrange
            var activity = new Activity("test");
            activity.Start();
            Activity.Current = activity;

            // Act
            var result = _controller.Error();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeOfType<ErrorViewModel>();
            var model = viewResult.Model as ErrorViewModel;
            model.RequestId.Should().Be(activity.Id);
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

        [TestMethod]
        public async Task GetRegistrationDataAsync_FormatsStatusCorrectly()
        {
            // Arrange
            var registrationData = new List<RegistrationDto>
            {
                new(){
                RegistrationId = Guid.NewGuid(),
                RegistrationStatus = (App.Enums.Registration.RegistrationStatus)RegistrationStatus.InProgress,
                AccreditationStatus = (App.Enums.Accreditation.AccreditationStatus)Epr.Reprocessor.Exporter.UI.Enums.AccreditationStatus.NotAccredited,
                ApplicationType = "Reprocessor",
                Year = 2025,
                ApplicationTypeId = 1,
                MaterialId = 1,
                Material = "Steel",
                ReprocessingSiteId = 1,
                RegistrationMaterialId = 1,
                ReprocessingSiteAddress = new AddressDto() {
                    Id = 1,
                    AddressLine1 = "12 leylands Road",
                    AddressLine2 = "Downing street",
                    TownCity = "Leeds"
                }
               }
            };

            _mockRegistrationService
                .Setup(x => x.GetRegistrationAndAccreditationAsync(It.IsAny<Guid?>()))
                .ReturnsAsync(registrationData);

            // Act
            var result = await _controller.ManageOrganisation();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            var model = viewResult.Model as HomeViewModel;
            model.RegistrationData[0].RegistrationStatus.Should().Be(RegistrationStatus.InProgress);
        }

        [TestMethod]
        public async Task GetAccreditationDataAsync_FormatsStatusCorrectly()
        {
            // Arrange
            var accreditationData = new List<RegistrationDto>
            {
                new(){
                RegistrationId = Guid.NewGuid(),
                RegistrationStatus = (App.Enums.Registration.RegistrationStatus)RegistrationStatus.InProgress,
                AccreditationStatus = (App.Enums.Accreditation.AccreditationStatus)Epr.Reprocessor.Exporter.UI.Enums.AccreditationStatus.Started,
                ApplicationType = "Reprocessor",
                Year = 2025,
                ApplicationTypeId = 1,
                MaterialId = 1,
                Material = "Steel",
                ReprocessingSiteId = 1,
                RegistrationMaterialId = 1,
                ReprocessingSiteAddress = new AddressDto() {
                    Id = 1,
                    AddressLine1 = "12 leylands Road",
                    AddressLine2 = "Downing street",
                    TownCity = "Leeds"
                }
               }
            };

            _mockRegistrationService
                .Setup(x => x.GetRegistrationAndAccreditationAsync(It.IsAny<Guid?>()))
                .ReturnsAsync(accreditationData);

            // Act
            var result = await _controller.ManageOrganisation();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            var model = viewResult.Model as HomeViewModel;
            model.AccreditationData[0].AccreditationStatus.Should().Be(Enums.AccreditationStatus.Started);
            model.AccreditationData[0].Action.Should().Contain("Continue");
        }
    }
}
