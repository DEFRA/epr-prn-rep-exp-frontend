using Epr.Reprocessor.Exporter.UI.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewComponents
{
    [TestClass]
    public class PrimaryNavigationViewComponentTests
    {
        private Mock<IOptions<ExternalUrlOptions>> _mockExternalUrlOptions;
        private Mock<IOptions<FrontEndAccountManagementOptions>> _mockFrontEndAccountManagementOptions;
        private Mock<HttpContext> _mockHttpContext;
        private PrimaryNavigationViewComponent _viewComponent;

        [TestInitialize]
        public void Setup()
        {
            _mockExternalUrlOptions = new Mock<IOptions<ExternalUrlOptions>>();
            _mockFrontEndAccountManagementOptions = new Mock<IOptions<FrontEndAccountManagementOptions>>();
            _mockHttpContext = new Mock<HttpContext>();

            var externalUrlOptions = new ExternalUrlOptions { LandingPage = "http://example.com/landing" };
            var frontEndAccountManagementOptions = new FrontEndAccountManagementOptions { BaseUrl = "http://example.com/account" };

            _mockExternalUrlOptions.Setup(x => x.Value).Returns(externalUrlOptions);
            _mockFrontEndAccountManagementOptions.Setup(x => x.Value).Returns(frontEndAccountManagementOptions);

            _viewComponent = new PrimaryNavigationViewComponent(_mockExternalUrlOptions.Object, _mockFrontEndAccountManagementOptions.Object);
            _viewComponent.ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new ViewContext
                {
                    HttpContext = _mockHttpContext.Object
                }
            };
        }

        [TestMethod]
        public void Invoke_UserDataIdIsNotNull_ReturnsViewWithPrimaryNavigationModel()
        {
            // Arrange
            var userData = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-id")
            }, "mock"));

            _mockHttpContext.Setup(x => x.User).Returns(userData);
            _mockHttpContext.Setup(x => x.Request.Path).Returns(new PathString("/"));

            // Act
            var result = _viewComponent.Invoke() as ViewViewComponentResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.ViewData.Model as PrimaryNavigationModel;
            Assert.IsNotNull(model);  
        }

        [TestMethod]
        public void Invoke_UserDataIdIsNull_ReturnsViewWithEmptyPrimaryNavigationModel()
        {
            // Arrange
            var userData = new ClaimsPrincipal(new ClaimsIdentity());

            _mockHttpContext.Setup(x => x.User).Returns(userData);

            // Act
            var result = _viewComponent.Invoke() as ViewViewComponentResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.ViewData.Model as PrimaryNavigationModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Items.Count);
        }
    }
}
