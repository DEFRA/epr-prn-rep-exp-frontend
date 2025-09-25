using static System.Net.Mime.MediaTypeNames;

namespace Epr.Reprocessor.Exporter.Tests.ViewModels
{
    [TestClass]
    public class CookieDetailViewModelTests
    {
        [TestMethod]
        public void TestDefaultValues()
        {
            // Arrange
            var viewModel = new CookieDetailViewModel();

            // Act & Assert
            Assert.IsFalse(viewModel.CookiesAccepted);
            Assert.IsFalse(viewModel.ShowAcknowledgement);
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.CookiePolicyCookieName));
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.SessionCookieName));
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.AntiForgeryCookieName));
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.TsCookieName));
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.AuthenticationCookieName));
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.TempDataCookieName));
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.B2CCookieName));
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.CorrelationCookieName));
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.OpenIdCookieName));
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.GoogleAnalyticsDefaultCookieName));
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.GoogleAnalyticsAdditionalCookieName));
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.ReturnUrl));
        }

        [TestMethod]
        public void TestPropertySetters()
        {
            // Arrange
            var viewModel = new CookieDetailViewModel();

            // Act
            viewModel.CookiesAccepted = true;
            viewModel.ShowAcknowledgement = true;
            viewModel.CookiePolicyCookieName = "PolicyCookie";
            viewModel.SessionCookieName = "SessionCookie";
            viewModel.AntiForgeryCookieName = "AntiForgeryCookie";
            viewModel.TsCookieName = "TsCookie";
            viewModel.AuthenticationCookieName = "AuthCookie";
            viewModel.TempDataCookieName = "TempDataCookie";
            viewModel.B2CCookieName = "B2CCookie";
            viewModel.CorrelationCookieName = "CorrelationCookie";
            viewModel.OpenIdCookieName = "OpenIdCookie";
            viewModel.GoogleAnalyticsDefaultCookieName = "GADefaultCookie";
            viewModel.GoogleAnalyticsAdditionalCookieName = "GAAdditionalCookie";
            viewModel.ReturnUrl = "http://example.com";

            // Assert
            Assert.IsTrue(viewModel.CookiesAccepted);
            Assert.IsTrue(viewModel.ShowAcknowledgement);
            Assert.AreEqual("PolicyCookie", viewModel.CookiePolicyCookieName);
            Assert.AreEqual("SessionCookie", viewModel.SessionCookieName);
            Assert.AreEqual("AntiForgeryCookie", viewModel.AntiForgeryCookieName);
            Assert.AreEqual("TsCookie", viewModel.TsCookieName);
            Assert.AreEqual("AuthCookie", viewModel.AuthenticationCookieName);
            Assert.AreEqual("TempDataCookie", viewModel.TempDataCookieName);
            Assert.AreEqual("B2CCookie", viewModel.B2CCookieName);
            Assert.AreEqual("CorrelationCookie", viewModel.CorrelationCookieName);
            Assert.AreEqual("OpenIdCookie", viewModel.OpenIdCookieName);
            Assert.AreEqual("GADefaultCookie", viewModel.GoogleAnalyticsDefaultCookieName);
            Assert.AreEqual("GAAdditionalCookie", viewModel.GoogleAnalyticsAdditionalCookieName);
            Assert.AreEqual("http://example.com", viewModel.ReturnUrl);
        }
    }
}
