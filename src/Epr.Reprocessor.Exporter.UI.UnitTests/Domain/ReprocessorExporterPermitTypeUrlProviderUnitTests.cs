namespace Epr.Reprocessor.Exporter.UI.UnitTests.Domain;

[TestClass]
public class ReprocessorExporterPermitTypeUrlProviderUnitTests
{
    [TestMethod]
    [DataRow(PermitType.WasteExemption, PagePaths.ExemptionReferences)]
    [DataRow(PermitType.PollutionPreventionAndControlPermit, PagePaths.PpcPermit)]
    [DataRow(PermitType.WasteManagementLicence, PagePaths.WasteManagementLicense)]
    [DataRow(PermitType.InstallationPermit, PagePaths.InstallationPermit)]
    [DataRow(PermitType.EnvironmentalPermitOrWasteManagementLicence, PagePaths.EnvironmentalPermitOrWasteManagementLicence)]
    public void Url_EnsureCorrectUrl(PermitType permitType, string? expectedUrl)
    {
        // Act
        var result = ReprocessorExporterPermitTypeUrlProvider.Url(permitType);

        // Assert
        result.Should().BeEquivalentTo(expectedUrl);
    }

    [TestMethod]
    public void Url_InvalidValueShouldThrow()
    {
        // Act
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ReprocessorExporterPermitTypeUrlProvider.Url((PermitType)100));
    }
}