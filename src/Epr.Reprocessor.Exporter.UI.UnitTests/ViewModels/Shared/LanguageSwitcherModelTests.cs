using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;
using System.Globalization;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels.Shared;

[TestClass]
public class LanguageSwitcherModelTests
{
    [TestMethod]
    public void TestCurrentCultureProperty()
    {
        // Arrange
        var model = new LanguageSwitcherModel();
        var culture = new CultureInfo("en-US");

        // Act
        model.CurrentCulture = culture;

        // Assert
        Assert.AreEqual(culture, model.CurrentCulture);
    }

    [TestMethod]
    public void TestSupportedCulturesProperty()
    {
        // Arrange
        var model = new LanguageSwitcherModel();
        var cultures = new List<CultureInfo> { new CultureInfo("en-US"), new CultureInfo("fr-FR") };

        // Act
        model.SupportedCultures = cultures;

        // Assert
        CollectionAssert.AreEqual(cultures, model.SupportedCultures);
    }

    [TestMethod]
    public void TestReturnUrlProperty()
    {
        // Arrange
        var model = new LanguageSwitcherModel();
        var returnUrl = "/home/index";

        // Act
        model.ReturnUrl = returnUrl;

        // Assert
        Assert.AreEqual(returnUrl, model.ReturnUrl);
    }

    [TestMethod]
    public void TestShowLanguageSwitcherProperty()
    {
        // Arrange
        var model = new LanguageSwitcherModel();
        var showLanguageSwitcher = true;

        // Act
        model.ShowLanguageSwitcher = showLanguageSwitcher;

        // Assert
        Assert.AreEqual(showLanguageSwitcher, model.ShowLanguageSwitcher);
    }
}
