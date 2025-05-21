using System.Security.Claims;
using System.Text.Json;
using EPR.Common.Authorization.Models;
using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.App.Options;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Middleware;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Middleware;

[TestClass]
public class UserDataCheckerMiddlewareTests
{
    private Mock<RequestDelegate> _requestDelegateMock = null!;
    private Mock<HttpRequest> _httpRequestMock = null!;
    private Mock<IRequestCookieCollection> _requestCookiesMock = null!;
    private Mock<HttpContext> _httpContextMock = null!;
    //private Mock<IFacadeService> _facadeServiceMock = null!;
    private Mock<IConfiguration> _configurationMock = null!;
    //private Mock<ISessionManager<JourneySession>> _sessionManagerMock = null!;
    private Mock<IUserAccountService> _userAccountServiceMock = null!;
    private UserDataCheckerMiddleware _systemUnderTest;

    [TestInitialize]
    public void Setup()
    {
        var userData = new UserData
        {
            Id = Guid.NewGuid(),
            Organisations = [new()]
        };

        var userDataString = JsonSerializer.Serialize(userData);
        var identity = new ClaimsIdentity();
        identity.AddClaims([
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim(ClaimTypes.UserData, userDataString)
        ]);
        var user = new ClaimsPrincipal(identity);

        _requestCookiesMock = new Mock<IRequestCookieCollection>();
        _httpRequestMock = new Mock<HttpRequest>();
        _httpRequestMock.Setup(x => x.Cookies).Returns(_requestCookiesMock.Object);

        _httpContextMock = new Mock<HttpContext>();
        _httpContextMock.Setup(x => x.User).Returns(user);

        _requestDelegateMock = new Mock<RequestDelegate>();
        //_facadeServiceMock = new Mock<IFacadeService>();

        _configurationMock = new Mock<IConfiguration>();
        //_sessionManagerMock = new Mock<ISessionManager<JourneySession>>();
        _userAccountServiceMock = new Mock<IUserAccountService>();

        SetupControllerName("UserDataControllerName");

        _systemUnderTest = new UserDataCheckerMiddleware( 
            new OptionsWrapper<FrontEndAccountCreationOptions>(new FrontEndAccountCreationOptions()),
            _userAccountServiceMock.Object,
            new Mock<ILogger<UserDataCheckerMiddleware>>().Object
        );
    } 

    [TestMethod]
    public async Task GivenInvokeAsync_WhenHomePathAndNotAuthenticated_ThenNoError()
    {
        // Arrange
        _httpRequestMock.Setup(x => x.Path).Returns("/" + PagePaths.UpdateCookieAcceptance);
        _httpContextMock.Setup(x => x.Request).Returns(_httpRequestMock.Object);
        _httpContextMock.Setup(x => x.User!.Identity!.IsAuthenticated).Returns(false);

        // Act
        await _systemUnderTest.InvokeAsync(_httpContextMock.Object, _requestDelegateMock.Object);

        // Assert
        _requestDelegateMock.Verify(x => x(_httpContextMock.Object), Times.Once);
    }

    [TestMethod]
    public async Task GivenInvokeAsync_IsAuthenticated_NoUserData()
    {
        // Arrange
        var mockAuthenticationServiceMock = new Mock<IAuthenticationService>();
        var serviceProvider = new ServiceCollection()
            .AddTransient(_ => mockAuthenticationServiceMock.Object)
            .BuildServiceProvider();

        var userData = new UserData
        {
            FirstName = "first",
            LastName = "last",
            Email = "email",
            EnrolmentStatus = "enrolled",
            RoleInOrganisation = "admin",
            Service = "service",
            ServiceRole = "role",
            ServiceRoleId = 1,
            Id = Guid.Empty,
            Organisations =
            [
                new()
                {
                    Id = Guid.Empty,
                    Name = "name",
                    OrganisationRole = "producer",
                    Town = "town",
                    BuildingName = "building name",
                    BuildingNumber = "building number",
                    Street = "street",
                    Locality = "locality",
                    County = "county",
                    Postcode = "postcode",
                    CompaniesHouseNumber = "companies house number",
                    OrganisationType = "organisation type",
                    Country = "country",
                    NationId = 1,
                    DependentLocality = "dependent locality",
                    JobTitle = "job title",
                    SubBuildingName = "sub building name"
                }
            ]
        };

        var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // Expectations
        _httpRequestMock.Setup(x => x.Path).Returns("/" + PagePaths.UpdateCookieAcceptance);
        _httpContextMock.Setup(x => x.Request).Returns(_httpRequestMock.Object);
        _httpContextMock.Setup(x => x.User).Returns(claimsPrincipal);
        _httpContextMock.Setup(x => x.RequestServices).Returns(serviceProvider);
        mockAuthenticationServiceMock
            .Setup(x => x.SignInAsync(_httpContextMock.Object, "scheme", claimsPrincipal,
                new AuthenticationProperties())).Returns(Task.CompletedTask);

        _userAccountServiceMock.Setup(o => o.GetUserAccount()).ReturnsAsync(new UserAccountDto()
        {
            User = new()
            {
                FirstName = "first",
                LastName = "last",
                Email = "email",
                EnrolmentStatus = "enrolled",
                Id = Guid.Empty,
                RoleInOrganisation = "admin",
                Service = "service",
                ServiceRole = "role",
                ServiceRoleId = 1,
                Organisations =
                [
                    new()
                    {
                        Id = Guid.Empty,
                        OrganisationName = "name",
                        OrganisationRole = "producer",
                        Town = "town",
                        BuildingName = "building name",
                        BuildingNumber = "building number",
                        Street = "street",
                        Locality = "locality",
                        County = "county",
                        Postcode = "postcode",
                        CompaniesHouseNumber = "companies house number",
                        OrganisationType = "organisation type",
                        Country = "country",
                        NationId = UkNation.England,
                        DependentLocality = "dependent locality",
                        JobTitle = "job title",
                        SubBuildingName = "sub building name",
                        OrganisationAddress = "address"
                    }
                ]
            }
        });

        // Act
        await _systemUnderTest.InvokeAsync(_httpContextMock.Object, _requestDelegateMock.Object);

        // Assert
        _requestDelegateMock.Verify(x => x(_httpContextMock.Object), Times.Once);

        // Assert
        claimsPrincipal.Claims.First().Value.Should().BeEquivalentTo(JsonSerializer.Serialize(userData));
    }

    //[TestMethod]
    //public async Task GivenInvokeAsync_WhenInvalidHomePathAndAuthenticated_ThenNoError()
    //{
    //    // Arrange            
    //    //_httpRequestMock.Setup(x => x.Path).Returns("/home");

    //    //var authenticationServiceMock = new Mock<IAuthenticationService>();
    //    //authenticationServiceMock.Setup(x => x.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>())).Returns(Task.CompletedTask);

    //    //var serviceProviderMock = new Mock<IServiceProvider>();
    //    //serviceProviderMock.Setup(x => x.GetService(typeof(IAuthenticationService))).Returns(authenticationServiceMock.Object);

    //    //_httpContextMock.SetupGet(x => x.RequestServices).Returns(serviceProviderMock.Object);
    //    //_httpContextMock.Setup(x => x.Request).Returns(_httpRequestMock.Object);
    //    //_httpContextMock.Setup(x => x.User!.Identity!.IsAuthenticated).Returns(true);

    //    //var responseList = new List<OrganisationResponse>
    //    //{
    //    //    new OrganisationResponse
    //    //    {
    //    //        Id = Guid.NewGuid().ToString(),
    //    //        Name = "org.Name",
    //    //        OrganisationRole = "org.OrganisationRole",
    //    //        OrganisationType = "org.OrganisationType",
    //    //        NationId = 1
    //    //    }
    //    //};

    //    //var content = new UserDataResponse
    //    //{
    //    //    UserDetails = new UserDetails
    //    //    {
    //    //        Id = Guid.NewGuid().ToString(),
    //    //        FirstName = "FirstName",
    //    //        LastName = "LastName",
    //    //        Email = "test@testing.com",
    //    //        RoleInOrganisation = "RoleInOrganisation",
    //    //        EnrolmentStatus = "EnrolmentStatus",
    //    //        ServiceRole = "ServiceRole",
    //    //        Service = "Service",
    //    //        ServiceRoleId = 0,
    //    //        Organisations = responseList
    //    //    }
    //    //};

    //    //var httpResponseMessage = new HttpResponseMessage
    //    //{
    //    //    StatusCode = HttpStatusCode.OK,
    //    //    Content = new StringContent(JsonSerializer.Serialize(content))
    //    //};

    //    //_facadeServiceMock.Setup(x => x.GetUserAccountDetails()).ReturnsAsync(httpResponseMessage);

    //    //// Act
    //    //await _systemUnderTest.InvokeAsync(_httpContextMock.Object, _requestDelegateMock.Object);

    //    //// Assert
    //    //_requestDelegateMock.Verify(x => x(_httpContextMock.Object), Times.Once);
    //    //_sessionManagerMock.Verify(x => x.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<JourneySession>()), Times.Once);

    //    //httpResponseMessage.Dispose();
    //}

    private void SetupControllerName(string controllerName)
    {
        var controllerActionDescriptor = new ControllerActionDescriptor { ControllerName = controllerName };

        var metadata = new List<object> { controllerActionDescriptor };

        _httpContextMock.Setup(x => x.Features.Get<IEndpointFeature>()!.Endpoint).Returns(new Endpoint(_ => Task.CompletedTask, new EndpointMetadataCollection(metadata), "EndpointName"));
    }
}