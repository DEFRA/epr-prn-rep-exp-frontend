using AutoFixture;
using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.Controllers;
using Epr.Reprocessor.Exporter.UI.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.Tests.Controllers
{
    [TestClass]
    public class RegistrationControllerTests
    {
        private RegistrationController _controller;
        private Fixture _fixture;
        private Mock<ILogger<RegistrationController>> _logger;
        private Mock<IUserJourneySaveAndContinueService> _userJourneySaveAndContinueService;

        [TestInitialize]
        public void Setup()
        {
            _logger = new Mock<ILogger<RegistrationController>>();
            _userJourneySaveAndContinueService = new Mock<IUserJourneySaveAndContinueService>();

            _controller = new RegistrationController(_logger.Object, _userJourneySaveAndContinueService.Object);

        }

        [TestMethod]
        public async Task UkSiteLocation_ShouldReturnView()
        {
            // Act
            var result = await _controller.UKSiteLocation();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [TestMethod]
        public async Task UkSiteLocation_OnSubmit_ShouldValidateModel()
        {
            var model = new UKSiteLocationViewModel() { SiteLocationId = null };

            ValidateViewModel(model);

            // Act
            var result = await _controller.UKSiteLocation(model);
            var modelState = _controller.ModelState;

            // Assert
            result.Should().BeOfType<ViewResult>();

            Assert.IsTrue(modelState["SiteLocationId"].Errors.Count == 1);
            Assert.AreEqual(modelState["SiteLocationId"].Errors[0].ErrorMessage, "Select the country the reprocessing site is located in.");
        }

        [TestMethod]
        public async Task UkSiteLocation_OnSubmit_ShouldRedirectNextPage()
        {
            var model = new UKSiteLocationViewModel() { SiteLocationId = Enums.UkNation.England };

            ValidateViewModel(model);

            // Act
            var result = await _controller.UKSiteLocation(model) as RedirectResult;

            // Assert
            result.Should().BeOfType<RedirectResult>();

            result.Url.Should().Be(PagePaths.PostcodeOfReprocessingSite);
        }

        [TestMethod]
        public async Task UkSiteLocation_OnSubmitSaveAndContinue_ShouldRedirectNextPage()
        {
            var model = new UKSiteLocationViewModel() { SiteLocationId = Enums.UkNation.England };
            var expectedModel = JsonConvert.SerializeObject(model);

            // Act
            var result = await _controller.UKSiteLocationSaveAndContinue(model) as RedirectResult;

            // Assert
            result.Should().BeOfType<RedirectResult>();
            result.Url.Should().Be(PagePaths.ApplicationSaved);

            _userJourneySaveAndContinueService.Verify(x=>x.SaveAndContinueAsync(nameof(RegistrationController.UKSiteLocation), nameof(RegistrationController), expectedModel), Times.Once);
            _userJourneySaveAndContinueService.VerifyNoOtherCalls();
        }


        private void ValidateViewModel(object Model)
        {
            ValidationContext validationContext = new ValidationContext(Model, null, null);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(Model, validationContext, validationResults, true);
            foreach (ValidationResult validationResult in validationResults)
            {
                _controller.ControllerContext.ModelState.AddModelError(String.Join(", ", validationResult.MemberNames), validationResult.ErrorMessage);
            }
        }
    }
}
