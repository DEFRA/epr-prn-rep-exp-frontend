using Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Domain;

[TestClass]
public class ReprocessorExporterTaskTypeUrlProviderUnitTests
{
    [TestMethod]
    [DataRow(TaskType.SiteAddressAndContactDetails, PagePaths.AddressOfReprocessingSite)]
    [DataRow(TaskType.WasteLicensesPermitsAndExemptions, PagePaths.WastePermitExemptions)]
    [DataRow(TaskType.SamplingAndInspectionPlan, PagePaths.RegistrationSamplingAndInspectionPlan)]
    [DataRow(TaskType.ReprocessingInputsAndOutputs, PagePaths.ReprocessingInputOutput)]
    [DataRow(TaskType.Unknown, null)]
    public void Url_EnsureCorrectUrl(TaskType taskType, string? expectedUrl)
    {
        // Act
        var result = ReprocessorExporterTaskTypeUrlProvider.Url(taskType);

        // Assert
        result.Should().BeEquivalentTo(expectedUrl);
    }

    [TestMethod]
    public void Url_InvalidValueShouldThrow()
    {
        // Act
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ReprocessorExporterTaskTypeUrlProvider.Url((TaskType)100));
    }
}