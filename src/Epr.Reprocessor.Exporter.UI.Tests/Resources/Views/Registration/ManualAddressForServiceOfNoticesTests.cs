using System.Globalization;
using System.Resources;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using FluentAssertions;

namespace Epr.Reprocessor.Exporter.UI.Tests.Resources.Views.Registration;

[TestClass]
public class ManualAddressForServiceOfNoticesTests
{
    private const string ResourceLocation = "Epr.Reprocessor.Exporter.UI.Resources.Views.Registration.ManualAddressForServiceOfNotices";
    private ResourceManager _resourceManager;

    [TestInitialize]
    public void Setup()
    {
        _resourceManager = new ResourceManager(ResourceLocation, typeof(ManualAddressForServiceOfNotices).Assembly);
    }

    [DataTestMethod]
    [DataRow("ManualAddressForServiceOfNoticesPageTitle")]
    [DataRow("AddressLine1")]
    [DataRow("AddressLine2Optional")]
    [DataRow("TownOrCity")]
    [DataRow("CountyOptional")]
    [DataRow("Postcode")]
    [DataRow("ValidationMessage_AddressLine1_Required")]
    [DataRow("ValidationMessage_AddressLine1_MaxLength")]
    [DataRow("ValidationMessage_AddressLine2_MaxLength")]
    [DataRow("ValidationMessage_TownOrCity_Required")]
    [DataRow("ValidationMessage_TownOrCity_MaxLength")]
    [DataRow("ValidationMessage_County_MaxLength")]
    [DataRow("ValidationMessage_Postcode_Required")]
    [DataRow("ValidationMessage_Postcode_Invalid")]
    public void GivenResourceKeyExists_ResourceShouldReturnValue(string resourceKey)
    {
        // Arrange

        // Act
        var value = _resourceManager.GetString(resourceKey, CultureInfo.InvariantCulture);

        // Assert
        value.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public void GivenResourceKeyIsMissing_ResourceShouldReturnNull()
    {
        // Arrange
        var nonExistentKey = "NonExistingKey";

        // Act
        var value = _resourceManager.GetString(nonExistentKey, CultureInfo.CurrentUICulture);

        // Assert
        value.Should().BeNull();
    }

    [TestMethod]
    [DataRow("ManualAddressForServiceOfNoticesPageTitle", "en", "UK address for service of notices")]
    [DataRow("AddressLine1", "en", "Address line 1")]
    [DataRow("AddressLine2Optional", "en", "Address line 2 (optional)")]
    [DataRow("TownOrCity", "en", "Town or city")]
    [DataRow("CountyOptional", "en", "County (optional)")]
    [DataRow("Postcode", "en", "Postcode")]
    [DataRow("ValidationMessage_AddressLine1_Required", "en", "Enter address line 1, typically the building and street")]
    [DataRow("ValidationMessage_AddressLine1_MaxLength", "en", "Address line 1 must be 100 characters or less")]
    [DataRow("ValidationMessage_AddressLine2_MaxLength", "en", "Address line 2 must be 100 characters or less")]
    [DataRow("ValidationMessage_TownOrCity_Required", "en", "Enter town or city")]
    [DataRow("ValidationMessage_TownOrCity_MaxLength", "en", "Town or city must be 70 characters or less")]
    [DataRow("ValidationMessage_County_MaxLength", "en", "County must be 50 characters or less")]
    [DataRow("ValidationMessage_Postcode_Required", "en", "Enter postcode")]
    [DataRow("ValidationMessage_Postcode_Invalid", "en", "Enter a full UK postcode")]
    public void GivenTranslationExists_ResourceShouldTranslateCorrectly(string resourceKey, string culture, string expectedValue)
    {
        // Arrange
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

        // Act
        var value = _resourceManager.GetString(resourceKey, CultureInfo.CurrentUICulture);

        // Assert
        value.Should().Be(expectedValue);
    }

    [TestMethod]
    [DataRow("ManualAddressForServiceOfNoticesPageTitle", "es", "UK address for service of notices")]
    [DataRow("AddressLine1", "es", "Address line 1")]
    [DataRow("AddressLine2Optional", "es", "Address line 2 (optional)")]
    [DataRow("TownOrCity", "es", "Town or city")]
    [DataRow("CountyOptional", "es", "County (optional)")]
    [DataRow("Postcode", "es", "Postcode")]
    [DataRow("ValidationMessage_AddressLine1_Required", "es", "Enter address line 1, typically the building and street")]
    [DataRow("ValidationMessage_AddressLine1_MaxLength", "es", "Address line 1 must be 100 characters or less")]
    [DataRow("ValidationMessage_AddressLine2_MaxLength", "es", "Address line 2 must be 100 characters or less")]
    [DataRow("ValidationMessage_TownOrCity_Required", "es", "Enter town or city")]
    [DataRow("ValidationMessage_TownOrCity_MaxLength", "es", "Town or city must be 70 characters or less")]
    [DataRow("ValidationMessage_County_MaxLength", "es", "County must be 50 characters or less")]
    [DataRow("ValidationMessage_Postcode_Required", "es", "Enter postcode")]
    [DataRow("ValidationMessage_Postcode_Invalid", "es", "Enter a full UK postcode")]
    public void GivenTranslationDoesNotExist_ResourceShouldFallbackToDefaultLanguage(string resourceKey, string cultureNotAvailable, string expectedValue)
    {
        // Arrange
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureNotAvailable);

        // Act
        var value = _resourceManager.GetString(resourceKey, CultureInfo.CurrentUICulture);

        // Assert
        value.Should().Be(expectedValue);
    }
}