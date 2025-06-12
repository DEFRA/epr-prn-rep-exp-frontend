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
            Assert.IsNull(viewModel.CookiePolicyCookieName);
            Assert.IsNull(viewModel.SessionCookieName);
            Assert.IsNull(viewModel.AntiForgeryCookieName);
            Assert.IsNull(viewModel.TsCookieName);
            Assert.IsNull(viewModel.AuthenticationCookieName);
            Assert.IsNull(viewModel.TempDataCookieName);
            Assert.IsNull(viewModel.B2CCookieName);
            Assert.IsNull(viewModel.CorrelationCookieName);
            Assert.IsNull(viewModel.OpenIdCookieName);
            Assert.IsNull(viewModel.GoogleAnalyticsDefaultCookieName);
            Assert.IsNull(viewModel.GoogleAnalyticsAdditionalCookieName);
            Assert.IsNull(viewModel.ReturnUrl);
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
