using Epr.Reprocessor.Exporter.UI.Controllers;
using Epr.Reprocessor.Exporter.UI.Controllers.ControllerExtensions;

namespace Epr.Reprocessor.Exporter.UI.Tests.Controllers.ControllerExtensions;

[TestClass]
public class ControllerExtensionsTests
{  
    [TestMethod]
    public void RemoveControllerFromName_ShouldRemoveControllerSuffix()
    {
        // Arrange 
        string controllerName = "MyController";
        // Act
        var data = nameof(HomeController).RemoveControllerFromName();
        //string result = _controller.RemoveControllerFromName(controllerName); 
        // Assert
        Assert.AreEqual("Home", data);
    } 
}
