using Microsoft.VisualStudio.TestTools.UnitTesting;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;
using System.Collections.Generic;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels.Shared
{
    [TestClass]
    public class PrimaryNavigationModelTests
    {
        [TestMethod]
        public void Items_ShouldBeInitializedAsNull()
        {
            // Arrange
            var model = new PrimaryNavigationModel();

            // Act
            var items = model.Items;

            // Assert
            Assert.IsNull(items);
        }

        [TestMethod]
        public void Items_ShouldBeAbleToSetAndGet()
        {
            // Arrange
            var model = new PrimaryNavigationModel();
            var navigationItems = new List<NavigationModel>
            {
                new NavigationModel { LocalizerKey = "Home", LinkValue = "/", IsActive = true },
                new NavigationModel { LocalizerKey = "About", LinkValue = "/about", IsActive = false }
            };

            // Act
            model.Items = navigationItems;
            var items = model.Items;

            // Assert
            Assert.IsNotNull(items);
            Assert.AreEqual(2, items.Count);
            Assert.AreEqual("Home", items[0].LocalizerKey);
            Assert.AreEqual("/", items[0].LinkValue);
            Assert.IsTrue(items[0].IsActive);
            Assert.AreEqual("About", items[1].LocalizerKey);
            Assert.AreEqual("/about", items[1].LinkValue);
            Assert.IsFalse(items[1].IsActive);
        }
    }
}
