using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using Moq;
using Epr.Reprocessor.Exporter.UI.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers;

[TestClass]
public class HomeControllerTests
{
    private Mock<ILogger<HomeController>> _mockLogger;
    private HomeController _controller;

    [TestInitialize]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<HomeController>>();
        _controller = new HomeController(_mockLogger.Object);
    }

    [TestMethod]
    public void Index_ReturnsViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        Assert.IsInstanceOfType(result, typeof(ViewResult));
    }

    [TestMethod]
    public void Privacy_ReturnsViewResult()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        Assert.IsInstanceOfType(result, typeof(ViewResult));
    }

    [TestMethod]
    public void Error_ReturnsViewResultWithErrorViewModel()
    {
        // Arrange
        var activity = new Activity("test");
        activity.Start();
        Activity.Current = activity;

        // Act
        var result = _controller.Error() as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Model, typeof(ErrorViewModel));
        var model = result.Model as ErrorViewModel;
        Assert.AreEqual(activity.Id, model.RequestId);
    }
}
