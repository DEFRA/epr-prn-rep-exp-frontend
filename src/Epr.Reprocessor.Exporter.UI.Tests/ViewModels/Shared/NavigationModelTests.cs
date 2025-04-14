using Microsoft.VisualStudio.TestTools.UnitTesting;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

namespace Epr.Reprocessor.Exporter.Tests.ViewModels.Shared
{
    [TestClass]
    public class NavigationModelTests
    {
        [TestMethod]
        public void LocalizerKey_Property_SetAndGet()
        {
            // Arrange
            var model = new NavigationModel();
            var expectedValue = "TestKey";

            // Act
            model.LocalizerKey = expectedValue;
            var actualValue = model.LocalizerKey;

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestMethod]
        public void LinkValue_Property_SetAndGet()
        {
            // Arrange
            var model = new NavigationModel();
            var expectedValue = "TestLink";

            // Act
            model.LinkValue = expectedValue;
            var actualValue = model.LinkValue;

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestMethod]
        public void IsActive_Property_SetAndGet()
        {
            // Arrange
            var model = new NavigationModel();
            var expectedValue = true;

            // Act
            model.IsActive = expectedValue;
            var actualValue = model.IsActive;

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }
    }
}
