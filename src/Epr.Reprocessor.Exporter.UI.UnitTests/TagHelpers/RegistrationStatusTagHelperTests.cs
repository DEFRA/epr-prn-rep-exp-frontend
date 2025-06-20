namespace Epr.Reprocessor.Exporter.UI.UnitTests.TagHelpers;

[TestClass]
public class RegistrationStatusTagHelperTests : TagHelpersUnitTestBase
{
    [TestInitialize]
    public void CustomSetup()
    {
        SetModelMetadata<RegistrationStatus>();
    }

    [TestMethod]
    public void Process_RegistrationStatus_InProgress_AppendsCorrectClass()
    {
        // Arrange
        var registrationStatus = RegistrationStatus.InProgress;
        var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(MockModelMetaDataProvider.Object, MockModelMetadata.Object, registrationStatus));

        var tagHelper = new RegistrationStatusColourClassTagHelper
        {
            RegStatus = modelExpression
        };

        SetModelMetadata<RegistrationStatus>();
        var tagHelperContext = GenerateTagHelperContext("div");
        var tagHelperOutput = GenerateTagHelperOutput("div", new TagHelperAttributeList { { "class", "existing-class" } });

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        Assert.AreEqual("existing-class govuk-tag--blue", tagHelperOutput.Attributes["class"].Value.ToString());
    }

    [TestMethod]
    public void Process_RegistrationStatus_Completed_AppendsCorrectClass()
    {
        // Arrange
        var registrationStatus = RegistrationStatus.Completed;
        var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(MockModelMetaDataProvider.Object, MockModelMetadata.Object, registrationStatus));

        var tagHelper = new RegistrationStatusColourClassTagHelper
        {
            RegStatus = modelExpression
        };

        var tagHelperContext = GenerateTagHelperContext("div");
        var tagHelperOutput = GenerateTagHelperOutput("div", new TagHelperAttributeList { { "class", "existing-class" } });

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        Assert.IsTrue(tagHelperOutput.Attributes["class"].Value.ToString().Contains("govuk-tag--blue"));
    }

    [TestMethod]
    public void Process_RegistrationStatus_Submitted_AppendsCorrectClass()
    {
        // Arrange
        var registrationStatus = RegistrationStatus.Submitted;
        var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(MockModelMetaDataProvider.Object, MockModelMetadata.Object, registrationStatus));

        var tagHelper = new RegistrationStatusColourClassTagHelper
        {
            RegStatus = modelExpression
        };

        var tagHelperContext = GenerateTagHelperContext("div");
        var tagHelperOutput = GenerateTagHelperOutput("div", new TagHelperAttributeList { { "class", "existing-class" } });

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        Assert.IsTrue(tagHelperOutput.Attributes["class"].Value.ToString().Contains("govuk-tag--yellow"));
    }

    [TestMethod]
    public void Process_RegistrationStatus_RegulatorReviewing_AppendsCorrectClass()
    {
        // Arrange
        var registrationStatus = RegistrationStatus.RegulatorReviewing;
        var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(MockModelMetaDataProvider.Object, MockModelMetadata.Object, registrationStatus));

        var tagHelper = new RegistrationStatusColourClassTagHelper
        {
            RegStatus = modelExpression
        };

        var tagHelperContext = GenerateTagHelperContext("div");
        var tagHelperOutput = GenerateTagHelperOutput("div", new TagHelperAttributeList { { "class", "existing-class" } });

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        Assert.AreEqual("existing-class govuk-tag--pink", tagHelperOutput.Attributes["class"].Value.ToString());
    }

    [TestMethod]
    public void Process_RegistrationStatus_Queried_AppendsCorrectClass()
    {
        // Arrange
        var registrationStatus = RegistrationStatus.Queried;
        var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(MockModelMetaDataProvider.Object, MockModelMetadata.Object, registrationStatus));

        var tagHelper = new RegistrationStatusColourClassTagHelper
        {
            RegStatus = modelExpression
        };

        var tagHelperContext = GenerateTagHelperContext("div");
        var tagHelperOutput = GenerateTagHelperOutput("div", new TagHelperAttributeList { { "class", "existing-class" } });

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        Assert.IsTrue(tagHelperOutput.Attributes["class"].Value.ToString().Contains("govuk-tag--purple"));
    }

    [TestMethod]
    public void Process_RegistrationStatus_Updated_AppendsCorrectClass()
    {
        // Arrange
        var registrationStatus = RegistrationStatus.Updated;
        var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(MockModelMetaDataProvider.Object, MockModelMetadata.Object, registrationStatus));

        var tagHelper = new RegistrationStatusColourClassTagHelper
        {
            RegStatus = modelExpression
        };

        var tagHelperContext = GenerateTagHelperContext("div");
        var tagHelperOutput = GenerateTagHelperOutput("div", new TagHelperAttributeList { { "class", "existing-class" } });

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        Assert.AreEqual(tagHelperOutput.Attributes["class"].Value.ToString(), "existing-class");
    }

    [TestMethod]
    public void Process_RegistrationStatus_Refused_AppendsCorrectClass()
    {
        // Arrange
        var registrationStatus = RegistrationStatus.Refused;
        var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(MockModelMetaDataProvider.Object, MockModelMetadata.Object, registrationStatus));

        var tagHelper = new RegistrationStatusColourClassTagHelper
        {
            RegStatus = modelExpression
        };

        var tagHelperContext = GenerateTagHelperContext("div");
        var tagHelperOutput = GenerateTagHelperOutput("div", new TagHelperAttributeList { { "class", "existing-class" } });

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        Assert.AreEqual("existing-class govuk-tag--red", tagHelperOutput.Attributes["class"].Value.ToString());
    }

    [TestMethod]
    public void Process_RegistrationStatus_Granted_AppendsCorrectClass()
    {
        // Arrange
        var registrationStatus = RegistrationStatus.Granted;
        var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(MockModelMetaDataProvider.Object, MockModelMetadata.Object, registrationStatus));

        var tagHelper = new RegistrationStatusColourClassTagHelper
        {
            RegStatus = modelExpression
        };

        var tagHelperContext = GenerateTagHelperContext("div");
        var tagHelperOutput = GenerateTagHelperOutput("div", new TagHelperAttributeList { { "class", "existing-class" } });

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        Assert.IsTrue(tagHelperOutput.Attributes["class"].Value.ToString().Contains("govuk-tag--green"));
    }

    [TestMethod]
    public void Process_RegistrationStatus_RenewalInProgress_AppendsCorrectClass()
    {
        // Arrange
        var registrationStatus = RegistrationStatus.RenewalInProgress;
        var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(MockModelMetaDataProvider.Object, MockModelMetadata.Object, registrationStatus));

        var tagHelper = new RegistrationStatusColourClassTagHelper
        {
            RegStatus = modelExpression
        };

        var tagHelperContext = GenerateTagHelperContext("div");
        var tagHelperOutput = GenerateTagHelperOutput("div", new TagHelperAttributeList { { "class", "existing-class" } });

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        Assert.AreEqual("existing-class govuk-tag--blue", tagHelperOutput.Attributes["class"].Value.ToString());
    }

    [TestMethod]
    public void Process_RegistrationStatus_RenewalSubmitted_AppendsCorrectClass()
    {
        // Arrange
        var registrationStatus = RegistrationStatus.RenewalSubmitted;
        var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(MockModelMetaDataProvider.Object, MockModelMetadata.Object, registrationStatus));

        var tagHelper = new RegistrationStatusColourClassTagHelper
        {
            RegStatus = modelExpression
        };

        var tagHelperContext = GenerateTagHelperContext("div");
        var tagHelperOutput = GenerateTagHelperOutput("div", new TagHelperAttributeList { { "class", "existing-class" } });

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        Assert.AreEqual("existing-class govuk-tag--yellow", tagHelperOutput.Attributes["class"].Value.ToString());
    }

    [TestMethod]
    public void Process_RegistrationStatus_RenewalQueried_AppendsCorrectClass()
    {
        // Arrange
        var registrationStatus = RegistrationStatus.RenewalQueried;
        var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(MockModelMetaDataProvider.Object, MockModelMetadata.Object, registrationStatus));

        var tagHelper = new RegistrationStatusColourClassTagHelper
        {
            RegStatus = modelExpression
        };

        var tagHelperContext = GenerateTagHelperContext("div");
        var tagHelperOutput = GenerateTagHelperOutput("div", new TagHelperAttributeList { { "class", "existing-class" } });

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        Assert.AreEqual("existing-class govuk-tag--purple", tagHelperOutput.Attributes["class"].Value.ToString());
    }

    [TestMethod]
    public void Process_RegistrationStatus_Suspended_AppendsCorrectClass()
    {
        // Arrange
        var registrationStatus = RegistrationStatus.Suspended;
        var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(MockModelMetaDataProvider.Object, MockModelMetadata.Object, registrationStatus));

        var tagHelper = new RegistrationStatusColourClassTagHelper
        {
            RegStatus = modelExpression
        };

        var tagHelperContext = GenerateTagHelperContext("div");
        var tagHelperOutput = GenerateTagHelperOutput("div", new TagHelperAttributeList { { "class", "existing-class" } });

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        Assert.AreEqual("existing-class govuk-tag--red", tagHelperOutput.Attributes["class"].Value.ToString());
    }

    [TestMethod]
    public void Process_RegistrationStatus_Cancelled_AppendsCorrectClass()
    {
        // Arrange
        var registrationStatus = RegistrationStatus.Cancelled;
        var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(MockModelMetaDataProvider.Object, MockModelMetadata.Object, registrationStatus));

        var tagHelper = new RegistrationStatusColourClassTagHelper
        {
            RegStatus = modelExpression
        };

        var tagHelperContext = GenerateTagHelperContext("div");
        var tagHelperOutput = GenerateTagHelperOutput("div", new TagHelperAttributeList { { "class", "existing-class" } });

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        Assert.AreEqual("existing-class", tagHelperOutput.Attributes["class"].Value.ToString());
    }

    [TestMethod]
    public void Process_RegistrationStatus_NeedsToBeRenewed_AppendsCorrectClass()
    {
        // Arrange
        var registrationStatus = RegistrationStatus.NeedsToBeRenewed;
        var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(MockModelMetaDataProvider.Object, MockModelMetadata.Object, registrationStatus));

        var tagHelper = new RegistrationStatusColourClassTagHelper
        {
            RegStatus = modelExpression
        };

        var tagHelperContext = GenerateTagHelperContext("div");
        var tagHelperOutput = GenerateTagHelperOutput("div", new TagHelperAttributeList { { "class", "existing-class" } });

        // Act
        tagHelper.Process(tagHelperContext, tagHelperOutput);

        // Assert
        Assert.AreEqual("existing-class", tagHelperOutput.Attributes["class"].Value.ToString());
    }
}