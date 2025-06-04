using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Security.Claims;
using Epr.Reprocessor.Exporter.UI.Middleware;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.App.Options;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;
using Microsoft.AspNetCore.Authentication;
using EPR.Common.Authorization.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Middleware
{

    [TestClass]
    public class UserDataCheckerMiddlewareTests
    {
        private readonly FrontEndAccountCreationOptions _frontEndAccountCreationOptions = new()
        {
            CreateUser = "/CreateUser",
            AddOrganisation = "/AddOrganisation"
        };
        private Mock<ClaimsPrincipal> _claimsPrincipalMock;
        private Mock<HttpContext> _httpContextMock;
        private Mock<RequestDelegate> _requestDelegateMock;
        private Mock<IUserAccountService> _userAccountServiceMock;
        private Mock<ILogger<UserDataCheckerMiddleware>> _loggerMock;
        private Mock<ControllerActionDescriptor> _controllerActionDescriptor;
        private readonly ModuleOptions _moduleOptions = new()
        {
            ServiceKey = "Test"
        };
        private readonly UserData _userData = new UserData
        {
            FirstName = GetUserAccount().User.FirstName,
            LastName = GetUserAccount().User.LastName,
            Email = GetUserAccount().User.Email,
            Id = GetUserAccount().User.Id,
            Organisations = GetUserAccount().User.Organisations?.Select(x =>
                new EPR.Common.Authorization.Models.Organisation
                {
                    Id = x.Id,
                    Name = x.OrganisationName,
                    OrganisationNumber = x.OrganisationNumber,
                    OrganisationRole = x.OrganisationRole,
                    OrganisationType = x.OrganisationType
                }).ToList()
        };
        private UserDataCheckerMiddleware _systemUnderTest;

        [TestInitialize]
        public void SetUp()
        {
            _claimsPrincipalMock = new Mock<ClaimsPrincipal>();
            _requestDelegateMock = new Mock<RequestDelegate>();
            _httpContextMock = new Mock<HttpContext>();
            _loggerMock = new Mock<ILogger<UserDataCheckerMiddleware>>();
            _userAccountServiceMock = new Mock<IUserAccountService>();
            _controllerActionDescriptor = new Mock<ControllerActionDescriptor>();

            var metadata = new List<object> { _controllerActionDescriptor.Object };

            _httpContextMock.Setup(x => x.Features.Get<IEndpointFeature>().Endpoint).Returns(new Endpoint(c => Task.CompletedTask, new EndpointMetadataCollection(metadata), "TestController"));
            _systemUnderTest = new UserDataCheckerMiddleware(Options.Create(_frontEndAccountCreationOptions), _userAccountServiceMock.Object, _loggerMock.Object,Options.Create(_moduleOptions));
        }

        [TestMethod]
        public async Task Middleware_DoesNotCallUserAccountService_WhenUserIsNotAuthenticated()
        {
            // Arrange
            _claimsPrincipalMock.Setup(x => x.Identity.IsAuthenticated).Returns(false);
            _httpContextMock.Setup(x => x.User).Returns(_claimsPrincipalMock.Object);

            // Act
            await _systemUnderTest.InvokeAsync(_httpContextMock.Object, _requestDelegateMock.Object);

            // Assert
            _userAccountServiceMock.Verify(x => x.GetUserAccount(_moduleOptions.ServiceKey), Times.Never);
            _requestDelegateMock.Verify(x => x(_httpContextMock.Object), Times.Once);
        }

        [TestMethod]
        public async Task Middleware_DoesNotCallUserAccountService_WhenUserDataWithOrganisationAlreadyExistsInUserClaims()
        {
            // Arrange
            _claimsPrincipalMock.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            _claimsPrincipalMock.Setup(x => x.Claims).Returns(new List<Claim> { new(ClaimTypes.UserData, JsonSerializer.Serialize( _userData)) });
            _httpContextMock.Setup(x => x.User).Returns(_claimsPrincipalMock.Object);

            // Act
            await _systemUnderTest.InvokeAsync(_httpContextMock.Object, _requestDelegateMock.Object);

            // Assert
            _userAccountServiceMock.Verify(x => x.GetUserAccount(_moduleOptions.ServiceKey), Times.Never);
            _requestDelegateMock.Verify(x => x(_httpContextMock.Object), Times.Once);
        }

        [TestMethod]
        public async Task Middleware_CallsUserAccountServiceAndSignsIn_WhenUserDataDoesNotExistInUserClaims()
        {
            // Arrange
            var claims = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>(), "authenticationType"));
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IAuthenticationService))).Returns(Mock.Of<IAuthenticationService>());
            _httpContextMock.Setup(x => x.User).Returns(claims);
            _httpContextMock.Setup(x => x.RequestServices).Returns(serviceProviderMock.Object);
            _userAccountServiceMock.Setup(x => x.GetUserAccount(_moduleOptions.ServiceKey)).ReturnsAsync(GetUserAccount());

            // Act
            await _systemUnderTest.InvokeAsync(_httpContextMock.Object, _requestDelegateMock.Object);

            // Assert
            _userAccountServiceMock.Verify(x => x.GetUserAccount(_moduleOptions.ServiceKey), Times.Once);
            _requestDelegateMock.Verify(x => x(_httpContextMock.Object), Times.Once);
        }

        [TestMethod]
        public async Task Middleware_CallsUserAccountServiceAndSignsIn_WhenUserDataExistButNoOrganisationInUserClaims()
        {
            // Arrange
            _claimsPrincipalMock.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            var claimWoOrganisation = new UserData() { FirstName = "Test" };
            _claimsPrincipalMock.Setup(x => x.Claims).Returns(new List<Claim> { new(ClaimTypes.UserData, JsonSerializer.Serialize(claimWoOrganisation)) });
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IAuthenticationService))).Returns(Mock.Of<IAuthenticationService>());
            _httpContextMock.Setup(x => x.RequestServices).Returns(serviceProviderMock.Object);
            _httpContextMock.Setup(x => x.User).Returns(_claimsPrincipalMock.Object);
            _userAccountServiceMock.Setup(x => x.GetUserAccount(_moduleOptions.ServiceKey)).ReturnsAsync(GetUserAccount());

            // Act
            await _systemUnderTest.InvokeAsync(_httpContextMock.Object, _requestDelegateMock.Object);

            // Assert
            _userAccountServiceMock.Verify(x => x.GetUserAccount(_moduleOptions.ServiceKey), Times.Once);
            _requestDelegateMock.Verify(x => x(_httpContextMock.Object), Times.Once);
        }
        [TestMethod]
        public async Task Middleware_RedirectToFrontendAccountCreation_WhenUserAccountServiceDoesNotReturnDataForUser()
        {
            // Arrange
            var httpResponseMock = new Mock<HttpResponse>();
            _claimsPrincipalMock.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            _httpContextMock.Setup(x => x.User).Returns(_claimsPrincipalMock.Object);
            _httpContextMock.Setup(x => x.Response).Returns(httpResponseMock.Object);

            // Act
            await _systemUnderTest.InvokeAsync(_httpContextMock.Object, _requestDelegateMock.Object);

            // Assert
            _userAccountServiceMock.Verify(x => x.GetUserAccount(_moduleOptions.ServiceKey), Times.Once);
            httpResponseMock.Verify(x => x.Redirect(_frontEndAccountCreationOptions.CreateUser), Times.Once);
            _requestDelegateMock.Verify(x => x(_httpContextMock.Object), Times.Never);
        }

        [TestMethod]
        public async Task Middleware_CallsUserAccountServiceAndSignsIn_WhenUserDataDoesNotExistInUserClaims_WithNullEndpoint()
        {
            // Arrange
            var claims = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>(), "authenticationType"));
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IAuthenticationService))).Returns(Mock.Of<IAuthenticationService>());
            _httpContextMock.Setup(x => x.User).Returns(claims);
            _httpContextMock.Setup(x => x.Features.Get<IEndpointFeature>().Endpoint).Returns((Endpoint)null);
            _httpContextMock.Setup(x => x.RequestServices).Returns(serviceProviderMock.Object);
            _userAccountServiceMock.Setup(x => x.GetUserAccount(_moduleOptions.ServiceKey)).ReturnsAsync(GetUserAccount());

            // Act
            await _systemUnderTest.InvokeAsync(_httpContextMock.Object, _requestDelegateMock.Object);

            // Assert
            _userAccountServiceMock.Verify(x => x.GetUserAccount(_moduleOptions.ServiceKey), Times.Once);
            _requestDelegateMock.Verify(x => x(_httpContextMock.Object), Times.Once);
        }

        private static UserAccountDto GetUserAccount()
        {
            return new UserAccountDto
            {
                User = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Joe",
                    LastName = "Test",
                    Email = "JoeTest@something.com",
                    RoleInOrganisation = "Test Role",
                    EnrolmentStatus = "Enrolled",
                    ServiceRole = "Test service role",
                    Service = "Test service",
                    Organisations = new List<App.DTOs.UserAccount.Organisation>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        OrganisationName = "TestCo",
                        OrganisationRole = "reprocessor",
                        OrganisationType = "test type",
                    },
                },
                },
            };
        }
    }
}