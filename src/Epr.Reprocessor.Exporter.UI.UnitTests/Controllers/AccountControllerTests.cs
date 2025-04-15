using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.Controllers;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using System.Text.Json;
using AutoFixture;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers;

[TestClass]
public class AccountControllerTests
{
    private AccountController _systemUnderTest = default!;
    private Mock<IOptionsMonitor<MicrosoftIdentityOptions>> _microsoftIdentityOptionsMonitor = null!;
    private Mock<IUrlHelper> _mockUrlHelperMock = null!;
    private Mock<ISession> _mockSession = null!;
	private Fixture _fixture;

	private readonly string _scheme = OpenIdConnectDefaults.AuthenticationScheme;

    [TestInitialize]
    public void Setup()
    {
        var au = new MicrosoftIdentityOptions();
        _microsoftIdentityOptionsMonitor = new Mock<IOptionsMonitor<MicrosoftIdentityOptions>>();
        _microsoftIdentityOptionsMonitor.Setup(x => x.CurrentValue).Returns(au);
        _microsoftIdentityOptionsMonitor.Setup(x => x.Get(_scheme)).Returns(new MicrosoftIdentityOptions { ResetPasswordPolicyId = "ResetPasswordPolicyId" });
        _fixture = new Fixture();
		_mockUrlHelperMock = new Mock<IUrlHelper>();
        _mockSession = new Mock<ISession>();

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Custom-Header"] = "88-test-tcb";
        httpContext.Request.Scheme = _scheme;
        httpContext.Session = _mockSession.Object;

        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext,
        };

        _systemUnderTest = new AccountController()
        {
            Url = _mockUrlHelperMock.Object,
            ControllerContext = controllerContext,
        };
    }

    [TestMethod]
    public void WhenSignIn_WithInvalidLocalUri_ThenReturnLocalUri()
    {
        // Arrange
        string redirectUri = "http://www.google.co.uk";
        string expectedUri = "~/";

        _mockUrlHelperMock
                .Setup(m => m.IsLocalUrl(It.IsAny<string>()))
               .Returns(false)
               .Verifiable();

        _mockUrlHelperMock
            .Setup(m => m.Content(It.IsAny<string>()))
           .Returns(expectedUri)
           .Verifiable();

        // Act
        var result = _systemUnderTest.SignIn(_scheme, redirectUri);

        // Assert
        Assert.IsNotNull(result);
        result.Should().BeOfType<ChallengeResult>();
        var response = result as ChallengeResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(expected: _scheme, actual: response.AuthenticationSchemes[0]);
        Assert.IsNotNull(response.Properties);
        Assert.AreEqual(expected: expectedUri, actual: response.Properties.RedirectUri);
    }

    [TestMethod]
    public void WhenSignIn_WithValidLocalUri_ThenReturnPassedInUri()
    {
        // Arrange
        string redirectUri = "~/home";

        _mockUrlHelperMock
                .Setup(m => m.IsLocalUrl(It.IsAny<string>()))
               .Returns(true)
               .Verifiable();

        _mockUrlHelperMock
            .Setup(m => m.Content(It.IsAny<string>()))
           .Returns(redirectUri)
           .Verifiable();

        // Act
        var result = _systemUnderTest.SignIn(_scheme, redirectUri);

        // Assert
        Assert.IsNotNull(result);
        result.Should().BeOfType<ChallengeResult>();
        var response = result as ChallengeResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(expected: _scheme, actual: response.AuthenticationSchemes[0]);
        Assert.IsNotNull(response.Properties);
        Assert.AreEqual(expected: redirectUri, actual: response.Properties.RedirectUri);
    } 

    [TestMethod]
    public void WhenSignOut_WithInvalidLocalUri_ThenReturnLocalUri()
    {
        // Act
        var result = _systemUnderTest.SignOut(_scheme);

        // Assert
        Assert.IsNotNull(result);
        result.Should().BeOfType<SignOutResult>();
        var response = result as SignOutResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(expected: 2, actual: response.AuthenticationSchemes.Count);
        Assert.IsNotNull(response.Properties);
    }

    [TestMethod]
    [DataRow(null, "culture", false, DisplayName = "Null")]
    [DataRow("", "culture", false, DisplayName = "Empty")]
    [DataRow(Language.English, $"\"culture\":\"{Language.English}\"", true, DisplayName = "Lang")]
    public void WhenSignOut_WithSelectedCulture_ThenIncludeCultureInRedirect(
        string culture,
        string searchString,
        bool expectedResult)
    {
        // Arrange
        byte[]? outCulture = culture != null ? Encoding.UTF8.GetBytes(culture) : null;
        _mockSession.Setup(x => x.TryGetValue(Language.SessionLanguageKey, out outCulture));

        _mockUrlHelperMock
           .Setup(m => m.Action(It.IsAny<UrlActionContext>()))
           .Returns((UrlActionContext c) => JsonSerializer.Serialize(c.Values));

        // Act
        var result = _systemUnderTest.SignOut(_scheme);

        // Assert
        Assert.IsNotNull(result);
        result.Should().BeOfType<SignOutResult>();
        var response = result as SignOutResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(expected: 2, actual: response.AuthenticationSchemes.Count);
        Assert.IsNotNull(response.Properties);
    }
	[TestMethod]
	public void SignIn_WhenCalled_ReturnsChallengeResult()
	{
		// Arrange
		var scheme = _fixture.Create<string>();
		var redirectUri = _fixture.Create<string>();
		// Act
		var result = _systemUnderTest.SignIn(scheme, redirectUri);
		// Assert
		result.Should().BeOfType<ChallengeResult>();
	}
	[TestMethod]
	public void SignIn_WhenRedirectUriIsEmpty_SetsDefaultRedirectUri()
	{
		// Arrange
		var scheme = _fixture.Create<string>();
		var redirectUri = string.Empty;
		// Act
		var result = _systemUnderTest.SignIn(scheme, redirectUri) as ChallengeResult;
		// Assert
		result.Properties.RedirectUri.Should().BeNull();
	}
}