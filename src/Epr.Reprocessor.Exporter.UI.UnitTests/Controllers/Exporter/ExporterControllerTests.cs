using System;
using System.Threading.Tasks;
using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;
using Epr.Reprocessor.Exporter.UI.App.Services;
using Epr.Reprocessor.Exporter.UI.Controllers.Exporter;
using Epr.Reprocessor.Exporter.UI.ViewModels.Reprocessor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers.Exporter;


[TestClass]
public class ExporterControllerTests
{
    private Mock<ILogger<ExporterController>> _loggerMock;
    private Mock<ISessionManager<ExporterRegistrationSession>> _sessionManagerMock;
    private Mock<IReprocessorService> _reprocessorServiceMock;
    private ExporterController _controller;

    [TestInitialize]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<ExporterController>>();
        _sessionManagerMock = new Mock<ISessionManager<ExporterRegistrationSession>>();
        _reprocessorServiceMock = new Mock<IReprocessorService>();

        _controller = new ExporterController(_loggerMock.Object, _sessionManagerMock.Object, _reprocessorServiceMock.Object);
        
        var context = new DefaultHttpContext();
        var sessionMock = new Mock<ISession>();

        sessionMock.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<byte[]>()));
        sessionMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny)).Returns(false);
        sessionMock.Setup(x => x.Remove(It.IsAny<string>()));

        context.Session = sessionMock.Object;
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = context
        };

        var sessionWithInterimSites = new ExporterRegistrationSession().CreateRegistration(Guid.NewGuid());
        sessionWithInterimSites.ExporterRegistrationApplicationSession.InterimSites = new InterimSites();

        _sessionManagerMock
            .Setup(s => s.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(sessionWithInterimSites);
    }

    [TestMethod]
    public async Task Get_InterimSitesQuestionOne_Returns_View_With_Model()
    {
        // Act
        var result = await _controller.InterimSitesQuestionOne();

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        Assert.AreEqual("InterimSitesQuestionOne", viewResult.ViewName);
        Assert.IsInstanceOfType(viewResult.Model, typeof(InterimSitesQuestionOneViewModel));
    }

    [TestMethod]
    public async Task Post_Invalid_Model_Returns_Same_View()
    {
        // Arrange
        _controller.ModelState.AddModelError("HasInterimSites", "Required");

        var model = new InterimSitesQuestionOneViewModel();

        // Act
        var result = await _controller.InterimSitesQuestionOne(model, "SaveAndContinue");

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        Assert.AreEqual(model, viewResult.Model);
    }

    [TestMethod]
    public async Task Post_SaveAndContinue_With_HasInterimSites_True_Redirects()
    {
        // Arrange
        var model = new InterimSitesQuestionOneViewModel { HasInterimSites = true };
        var session = new ExporterRegistrationSession().CreateRegistration(Guid.NewGuid());
        session.ExporterRegistrationApplicationSession.InterimSites = new InterimSites();

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // Act
        var result = await _controller.InterimSitesQuestionOne(model, "SaveAndContinue");

        // Assert
        var redirectResult = result as RedirectResult;
        Assert.IsNotNull(redirectResult);
        Assert.IsTrue(redirectResult.Url.Contains("placeholder", StringComparison.OrdinalIgnoreCase));//needs updating once page exists
    }

    [TestMethod]
    public async Task Post_SaveAndContinue_With_HasInterimSites_False_Redirects()
    {
        // Arrange
        var model = new InterimSitesQuestionOneViewModel { HasInterimSites = false };
        var session = new ExporterRegistrationSession().CreateRegistration(Guid.NewGuid());
        session.ExporterRegistrationApplicationSession.InterimSites = new InterimSites();

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // Act
        var result = await _controller.InterimSitesQuestionOne(model, "SaveAndContinue");

        // Assert
        var redirectResult = result as RedirectResult;
        Assert.IsNotNull(redirectResult);
        Assert.IsTrue(redirectResult.Url.Contains("placeholder", StringComparison.OrdinalIgnoreCase));//needs updating once page exists
    }

    [TestMethod]
    public async Task Post_SaveAndComeBackLater_Redirects_To_ApplicationSaved()
    {
        // Arrange
        var model = new InterimSitesQuestionOneViewModel { HasInterimSites = true };
        var session = new ExporterRegistrationSession().CreateRegistration(Guid.NewGuid());
        session.ExporterRegistrationApplicationSession.InterimSites = new InterimSites();

        _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

        // Act
        var result = await _controller.InterimSitesQuestionOne(model, "SaveAndComeBackLater");

        // Assert
        var redirectResult = result as RedirectResult;
        Assert.IsNotNull(redirectResult);
        Assert.IsTrue(redirectResult.Url.Contains("application-saved", StringComparison.OrdinalIgnoreCase));//needs updating once page exists
    }

}