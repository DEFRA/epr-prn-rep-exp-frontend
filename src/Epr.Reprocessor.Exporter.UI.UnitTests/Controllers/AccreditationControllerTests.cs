using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.Controllers;
using Epr.Reprocessor.Exporter.UI.ViewModels.Reprocessor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Moq;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers
{
    [TestClass]
    public class AccreditationControllerTests
    {
        private AccreditationController _controller;
        private Mock<IStringLocalizer<SharedResources>> _mockLocalizer = new();

        [TestInitialize]
        public void Setup()
        {
            _controller = new AccreditationController(_mockLocalizer.Object);
        }

        [TestMethod]
        public async Task ApplicationSaved_ReturnsExpectedViewResult()
        {
            // Act
            var result =  _controller.ApplicationSaved();

            // Assert
            Assert.AreSame(typeof(ViewResult), result.GetType(), "Result should be of type ViewResult");
            
        }
    }
}
