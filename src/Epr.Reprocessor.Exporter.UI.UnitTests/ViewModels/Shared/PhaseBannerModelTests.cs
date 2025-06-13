namespace Epr.Reprocessor.Exporter.Tests.ViewModels.Shared
{
    [TestClass]
    public class PhaseBannerModelTests
    {
        [TestMethod]
        public void StatusProperty_ShouldGetAndSetCorrectly()
        {
            // Arrange
            var model = new PhaseBannerModel();
            var expectedStatus = "Test Status";

            // Act
            model.Status = expectedStatus;
            var actualStatus = model.Status;

            // Assert
            Assert.AreEqual(expectedStatus, actualStatus);
        }

        [TestMethod]
        public void UrlProperty_ShouldGetAndSetCorrectly()
        {
            // Arrange
            var model = new PhaseBannerModel();
            var expectedUrl = "http://example.com";

            // Act
            model.Url = expectedUrl;
            var actualUrl = model.Url;

            // Assert
            Assert.AreEqual(expectedUrl, actualUrl);
        }

        [TestMethod]
        public void ShowBannerProperty_ShouldGetAndSetCorrectly()
        {
            // Arrange
            var model = new PhaseBannerModel();
            var expectedShowBanner = true;

            // Act
            model.ShowBanner = expectedShowBanner;
            var actualShowBanner = model.ShowBanner;

            // Assert
            Assert.AreEqual(expectedShowBanner, actualShowBanner);
        }
    }
}
