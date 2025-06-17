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

    [TestMethod]
    public void Process_AccreditationStatus_Started_AppendsCorrectClass()
    {
        // Arrange
        var accreditationStatus = Enums.AccreditationStatus.Started;
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
        Assert.AreEqual("existing-class govuk-tag--blue", tagHelperOutput.Attributes["class"].Value.ToString());
    }

    [TestMethod]
    public void Process_AccreditationStatus_Submitted_AppendsCorrectClass()
    {
        // Arrange
        var accreditationStatus = Enums.AccreditationStatus.Submitted;
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
        Assert.AreEqual("existing-class govuk-tag--turquoise", tagHelperOutput.Attributes["class"].Value.ToString());
    }

    [TestMethod]
    public void Process_AccreditationStatus_Queried_AppendsCorrectClass()
    {
        // Arrange
        var accreditationStatus = Enums.AccreditationStatus.Queried;
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
        Assert.AreEqual("existing-class govuk-tag--purple", tagHelperOutput.Attributes["class"].Value.ToString());
    }

    [TestMethod]
    public void Process_AccreditationStatus_Updated_AppendsCorrectClass()
    {
        // Arrange
        var accreditationStatus = Enums.AccreditationStatus.Updated;
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
        Assert.AreEqual("existing-class", tagHelperOutput.Attributes["class"].Value.ToString());
    }

    [TestMethod]
    public void Process_AccreditationStatus_Refused_AppendsCorrectClass()
    {
        // Arrange
        var accreditationStatus = Enums.AccreditationStatus.Refused;
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
        Assert.AreEqual("existing-class govuk-tag--red", tagHelperOutput.Attributes["class"].Value.ToString());
    }

    [TestMethod]
    public void Process_AccreditationStatus_Granted_AppendsCorrectClass()
    {
        // Arrange
        var accreditationStatus = Enums.AccreditationStatus.Granted;
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
        Assert.AreEqual("existing-class govuk-tag--green", tagHelperOutput.Attributes["class"].Value.ToString());
    }

    [TestMethod]
    public void Process_AccreditationStatus_Accepted_AppendsCorrectClass()
    {
        // Arrange
        var accreditationStatus = Enums.AccreditationStatus.Accepted;
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
        Assert.AreEqual("existing-class govuk-tag--green", tagHelperOutput.Attributes["class"].Value.ToString());
    }

    [TestMethod]
    public void Process_AccreditationStatus_NotAccredited_AppendsCorrectClass()
    {
        // Arrange
        var accreditationStatus = Enums.AccreditationStatus.NotAccredited;
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
        Assert.AreEqual("existing-class govuk-tag--grey", tagHelperOutput.Attributes["class"].Value.ToString());
    }

    [TestMethod]
    public void Process_AccreditationStatus_Suspended_AppendsCorrectClass()
    {
        // Arrange
        var accreditationStatus = Enums.AccreditationStatus.Suspended;
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
        Assert.AreEqual("existing-class govuk-tag--red", tagHelperOutput.Attributes["class"].Value.ToString());
    }

    [TestMethod]
    public void Process_AccreditationStatus_Cancelled_AppendsCorrectClass()
    {
        // Arrange
        var accreditationStatus = Enums.AccreditationStatus.Cancelled;
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
        Assert.AreEqual("existing-class", tagHelperOutput.Attributes["class"].Value.ToString());
    }
} 