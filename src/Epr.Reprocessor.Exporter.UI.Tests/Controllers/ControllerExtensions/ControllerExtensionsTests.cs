using Epr.Reprocessor.Exporter.UI.Controllers;
using EPR.Common.Authorization.Sessions;
using Epr.Reprocessor.Exporter.UI.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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
