using Epr.Reprocessor.Exporter.UI.Enums;
using Epr.Reprocessor.Exporter.UI.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.TagHelpers;

[TestClass]
public class AccreditationStatusTagHelperTests : TagHelpersUnitTestBase
{
    [TestInitialize]
    public void CustomSetup()
    {
        SetModelMetadata<Enums.AccreditationStatus>();
    }

    [DataTestMethod]
    [DataRow(Enums.AccreditationStatus.Started, "existing-class govuk-tag--blue")]
    [DataRow(Enums.AccreditationStatus.Submitted, "existing-class govuk-tag--turquoise")]
    [DataRow(Enums.AccreditationStatus.Queried, "existing-class govuk-tag--purple")]
    [DataRow(Enums.AccreditationStatus.Updated, "existing-class")]
    [DataRow(Enums.AccreditationStatus.Refused, "existing-class govuk-tag--red")]
    [DataRow(Enums.AccreditationStatus.Granted, "existing-class govuk-tag--green")]
    [DataRow(Enums.AccreditationStatus.Accepted, "existing-class govuk-tag--green")]
    [DataRow(Enums.AccreditationStatus.NotAccredited, "existing-class govuk-tag--grey")]
    [DataRow(Enums.AccreditationStatus.Suspended, "existing-class govuk-tag--red")]
    [DataRow(Enums.AccreditationStatus.Cancelled, "existing-class")]
    public void Process_AccreditationStatus_AppendsCorrectClass(Enums.AccreditationStatus accreditationStatus, string expectedClass)
    {
        // Arrange
        var modelExpression = new ModelExpression("AccredStatus", new ModelExplorer(MockModelMetaDataProvider.Object, MockModelMetadata.Object, accreditationStatus));
        var tagHelper = new AccreditationStatusColourClassTagHelper
        {
            AccredStatus = modelExpression
        };
        var tagHelperContext = GenerateTagHelperContext("div");
        var tagHelperOutput = GenerateTagHelperOutput("div", new TagHelperAttributeList { { "class", "existing-class" } });

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        Assert.AreEqual(expectedClass, tagHelperOutput.Attributes["class"].Value.ToString());
    }
} 