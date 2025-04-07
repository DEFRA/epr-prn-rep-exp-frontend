using Epr.Reprocessor.Exporter.UI.App.Options;
using Epr.Reprocessor.Exporter.UI.ViewComponents;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Epr.Reprocessor.Exporter.UI.Tests.ViewComponents
{
    [TestClass]
    public class PhaseBannerViewComponentTests
    {
        private Mock<IOptions<PhaseBannerOptions>> _mockOptions;
        private PhaseBannerViewComponent _viewComponent;

        [TestInitialize]
        public void Setup()
        {
            _mockOptions = new Mock<IOptions<PhaseBannerOptions>>();
        }

        [TestMethod]
        public void Invoke_ShouldReturnViewWithCorrectModel_WhenOptionsAreSet()
        {
            // Arrange
            var options = new PhaseBannerOptions
            {
                ApplicationStatus = "Beta",
                SurveyUrl = "http://example.com/survey",
                Enabled = true
            };
            _mockOptions.Setup(o => o.Value).Returns(options);
            _viewComponent = new PhaseBannerViewComponent(_mockOptions.Object);

            // Act
            var result = _viewComponent.Invoke() as ViewViewComponentResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.ViewData.Model as PhaseBannerModel;
            Assert.IsNotNull(model);
            Assert.AreEqual("PhaseBanner.Beta", model.Status);
            Assert.AreEqual("http://example.com/survey", model.Url);
            Assert.IsTrue(model.ShowBanner);
        }

        [TestMethod]
        public void Invoke_ShouldReturnViewWithCorrectModel_WhenBannerIsDisabled()
        {
            // Arrange
            var options = new PhaseBannerOptions
            {
                ApplicationStatus = "Alpha",
                SurveyUrl = "http://example.com/survey",
                Enabled = false
            };
            _mockOptions.Setup(o => o.Value).Returns(options);
            _viewComponent = new PhaseBannerViewComponent(_mockOptions.Object);

            // Act
            var result = _viewComponent.Invoke() as ViewViewComponentResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.ViewData.Model as PhaseBannerModel;
            Assert.IsNotNull(model);
            Assert.AreEqual("PhaseBanner.Alpha", model.Status);
            Assert.AreEqual("http://example.com/survey", model.Url);
            Assert.IsFalse(model.ShowBanner);
        }
    }
}
