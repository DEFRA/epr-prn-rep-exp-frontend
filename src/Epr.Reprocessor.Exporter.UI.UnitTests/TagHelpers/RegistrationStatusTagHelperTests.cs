using Epr.Reprocessor.Exporter.UI.Controllers;
using Epr.Reprocessor.Exporter.UI.Enums;
using Epr.Reprocessor.Exporter.UI.TagHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.TagHelpers
{
    [TestClass]
    public class RegistrationStatusTagHelperTests
    {
        Mock<IModelMetadataProvider> metaDataProvider = new Mock<IModelMetadataProvider>();
        Mock<ModelMetadata> mockMetadata = new Mock<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(RegistrationStatus)));
        RegistrationStatus registrationStatus { get; set; } = RegistrationStatus.Completed;
        [TestInitialize]
        public void Setup()
        {
            metaDataProvider
                 .Setup(provider => provider.GetMetadataForType(typeof(Object)))
                 .Returns(mockMetadata.Object);
        }


        [TestMethod]
        public void Process_RegistrationStatus_InProgress_AppendsCorrectClass()
        {

            registrationStatus = RegistrationStatus.InProgress;
            var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(metaDataProvider.Object, mockMetadata.Object, registrationStatus));

            var tagHelper = new RegistrationStatusColourClassTagHelper
            {
                RegStatus = modelExpression
            };

            var tagHelperContext = new TagHelperContext(
                tagName: "div",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            var tagHelperOutput = new TagHelperOutput(
                "div",
                attributes: new TagHelperAttributeList { { "class", "existing-class" } },
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            Assert.AreEqual(tagHelperOutput.Attributes["class"].Value.ToString(), "existing-class");
        }

        [TestMethod]
        public void Process_RegistrationStatus_Completed_AppendsCorrectClass()
        {

            registrationStatus = RegistrationStatus.Completed;
            var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(metaDataProvider.Object, mockMetadata.Object, registrationStatus));

            var tagHelper = new RegistrationStatusColourClassTagHelper
            {
                RegStatus = modelExpression
            };

            var tagHelperContext = new TagHelperContext(
                tagName: "div",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            var tagHelperOutput = new TagHelperOutput(
                "div",
                attributes: new TagHelperAttributeList { { "class", "existing-class" } },
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            Assert.IsTrue(tagHelperOutput.Attributes["class"].Value.ToString().Contains("govuk-tag--blue"));
        }

        [TestMethod]
        public void Process_RegistrationStatus_Submitted_AppendsCorrectClass()
        {

            registrationStatus = RegistrationStatus.Submitted;
            var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(metaDataProvider.Object, mockMetadata.Object, registrationStatus));

            var tagHelper = new RegistrationStatusColourClassTagHelper
            {
                RegStatus = modelExpression
            };

            var tagHelperContext = new TagHelperContext(
                tagName: "div",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            var tagHelperOutput = new TagHelperOutput(
                "div",
                attributes: new TagHelperAttributeList { { "class", "existing-class" } },
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            Assert.IsTrue(tagHelperOutput.Attributes["class"].Value.ToString().Contains("govuk-tag--turquoise"));
        }



        [TestMethod]
        public void Process_RegistrationStatus_RegulatorReviewing_AppendsCorrectClass()
        {

            registrationStatus = RegistrationStatus.RegulatorReviewing;
            var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(metaDataProvider.Object, mockMetadata.Object, registrationStatus));

            var tagHelper = new RegistrationStatusColourClassTagHelper
            {
                RegStatus = modelExpression
            };

            var tagHelperContext = new TagHelperContext(
                tagName: "div",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            var tagHelperOutput = new TagHelperOutput(
                "div",
                attributes: new TagHelperAttributeList { { "class", "existing-class" } },
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            Assert.AreEqual(tagHelperOutput.Attributes["class"].Value.ToString(), "existing-class");
        }

        [TestMethod]
        public void Process_RegistrationStatus_Queried_AppendsCorrectClass()
        {

            registrationStatus = RegistrationStatus.Queried;
            var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(metaDataProvider.Object, mockMetadata.Object, registrationStatus));

            var tagHelper = new RegistrationStatusColourClassTagHelper
            {
                RegStatus = modelExpression
            };

            var tagHelperContext = new TagHelperContext(
                tagName: "div",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            var tagHelperOutput = new TagHelperOutput(
                "div",
                attributes: new TagHelperAttributeList { { "class", "existing-class" } },
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            Assert.IsTrue(tagHelperOutput.Attributes["class"].Value.ToString().Contains("govuk-tag--blue"));
        }


        [TestMethod]
        public void Process_RegistrationStatus_Updated_AppendsCorrectClass()
        {

            registrationStatus = RegistrationStatus.Updated;
            var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(metaDataProvider.Object, mockMetadata.Object, registrationStatus));

            var tagHelper = new RegistrationStatusColourClassTagHelper
            {
                RegStatus = modelExpression
            };

            var tagHelperContext = new TagHelperContext(
                tagName: "div",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            var tagHelperOutput = new TagHelperOutput(
                "div",
                attributes: new TagHelperAttributeList { { "class", "existing-class" } },
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            Assert.AreEqual(tagHelperOutput.Attributes["class"].Value.ToString(), "existing-class");
        }



        [TestMethod]
        public void Process_RegistrationStatus_Refused_AppendsCorrectClass()
        {

            registrationStatus = RegistrationStatus.Refused;
            var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(metaDataProvider.Object, mockMetadata.Object, registrationStatus));

            var tagHelper = new RegistrationStatusColourClassTagHelper
            {
                RegStatus = modelExpression
            };

            var tagHelperContext = new TagHelperContext(
                tagName: "div",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            var tagHelperOutput = new TagHelperOutput(
                "div",
                attributes: new TagHelperAttributeList { { "class", "existing-class" } },
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            Assert.AreEqual(tagHelperOutput.Attributes["class"].Value.ToString(), "existing-class");
        }






        [TestMethod]
        public void Process_RegistrationStatus_Granted_AppendsCorrectClass()
        {

            registrationStatus = RegistrationStatus.Granted;
            var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(metaDataProvider.Object, mockMetadata.Object, registrationStatus));

            var tagHelper = new RegistrationStatusColourClassTagHelper
            {
                RegStatus = modelExpression
            };

            var tagHelperContext = new TagHelperContext(
                tagName: "div",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            var tagHelperOutput = new TagHelperOutput(
                "div",
                attributes: new TagHelperAttributeList { { "class", "existing-class" } },
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            Assert.IsTrue(tagHelperOutput.Attributes["class"].Value.ToString().Contains("govuk-tag--green"));
        }




        [TestMethod]
        public void Process_RegistrationStatus_RenewalInProgress_AppendsCorrectClass()
        {

            registrationStatus = RegistrationStatus.RenewalInProgress;
            var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(metaDataProvider.Object, mockMetadata.Object, registrationStatus));

            var tagHelper = new RegistrationStatusColourClassTagHelper
            {
                RegStatus = modelExpression
            };

            var tagHelperContext = new TagHelperContext(
                tagName: "div",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            var tagHelperOutput = new TagHelperOutput(
                "div",
                attributes: new TagHelperAttributeList { { "class", "existing-class" } },
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            Assert.AreEqual(tagHelperOutput.Attributes["class"].Value.ToString(), "existing-class");
        }



        [TestMethod]
        public void Process_RegistrationStatus_RenewalSubmitted_AppendsCorrectClass()
        {

            registrationStatus = RegistrationStatus.RenewalSubmitted;
            var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(metaDataProvider.Object, mockMetadata.Object, registrationStatus));

            var tagHelper = new RegistrationStatusColourClassTagHelper
            {
                RegStatus = modelExpression
            };

            var tagHelperContext = new TagHelperContext(
                tagName: "div",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            var tagHelperOutput = new TagHelperOutput(
                "div",
                attributes: new TagHelperAttributeList { { "class", "existing-class" } },
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            Assert.AreEqual(tagHelperOutput.Attributes["class"].Value.ToString(), "existing-class");
        }

        [TestMethod]
        public void Process_RegistrationStatus_RenewalQueried_AppendsCorrectClass()
        {

            registrationStatus = RegistrationStatus.RenewalQueried;
            var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(metaDataProvider.Object, mockMetadata.Object, registrationStatus));

            var tagHelper = new RegistrationStatusColourClassTagHelper
            {
                RegStatus = modelExpression
            };

            var tagHelperContext = new TagHelperContext(
                tagName: "div",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            var tagHelperOutput = new TagHelperOutput(
                "div",
                attributes: new TagHelperAttributeList { { "class", "existing-class" } },
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            Assert.AreEqual(tagHelperOutput.Attributes["class"].Value.ToString(), "existing-class");
        }



        [TestMethod]
        public void Process_RegistrationStatus_Suspended_AppendsCorrectClass()
        {

            registrationStatus = RegistrationStatus.Suspended;
            var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(metaDataProvider.Object, mockMetadata.Object, registrationStatus));

            var tagHelper = new RegistrationStatusColourClassTagHelper
            {
                RegStatus = modelExpression
            };

            var tagHelperContext = new TagHelperContext(
                tagName: "div",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            var tagHelperOutput = new TagHelperOutput(
                "div",
                attributes: new TagHelperAttributeList { { "class", "existing-class" } },
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            Assert.AreEqual(tagHelperOutput.Attributes["class"].Value.ToString(), "existing-class");
        }

        [TestMethod]
        public void Process_RegistrationStatus_Cancelled_AppendsCorrectClass()
        {

            registrationStatus = RegistrationStatus.Cancelled;
            var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(metaDataProvider.Object, mockMetadata.Object, registrationStatus));

            var tagHelper = new RegistrationStatusColourClassTagHelper
            {
                RegStatus = modelExpression
            };

            var tagHelperContext = new TagHelperContext(
                tagName: "div",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            var tagHelperOutput = new TagHelperOutput(
                "div",
                attributes: new TagHelperAttributeList { { "class", "existing-class" } },
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            Assert.AreEqual(tagHelperOutput.Attributes["class"].Value.ToString(), "existing-class");
        }



        [TestMethod]
        public void Process_RegistrationStatus_NeedsToBeRenewed_AppendsCorrectClass()
        {

            registrationStatus = RegistrationStatus.NeedsToBeRenewed;
            var modelExpression = new ModelExpression("RegStatus", new ModelExplorer(metaDataProvider.Object, mockMetadata.Object, registrationStatus));

            var tagHelper = new RegistrationStatusColourClassTagHelper
            {
                RegStatus = modelExpression
            };

            var tagHelperContext = new TagHelperContext(
                tagName: "div",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");

            var tagHelperOutput = new TagHelperOutput(
                "div",
                attributes: new TagHelperAttributeList { { "class", "existing-class" } },
                getChildContentAsync: (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            Assert.AreEqual(tagHelperOutput.Attributes["class"].Value.ToString(), "existing-class");
        }

    }
}
