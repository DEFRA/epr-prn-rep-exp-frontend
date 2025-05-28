using System;
using System.Collections.Generic;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;
using Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.Enums.Accreditation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels.Accreditation
{
    [TestClass]
    public class AccreditationBaseViewModelTests
    {
        [TestMethod]
        public void Default_Constructor_Sets_Defaults()
        {
            var model = new AccreditationBaseViewModel();

            Assert.IsNull(model.Action);
            Assert.IsNull(model.Accreditation);
            Assert.IsNotNull(model.PrnIssueAuthorities);
            Assert.AreEqual(0, model.PrnIssueAuthorities.Count);
            Assert.AreEqual(ApplicationType.Reprocessor, model.ApplicationType);
            Assert.AreEqual("PRN", model.ApplicationTypeDescription);
        }

        [TestMethod]
        public void Setting_Accreditation_Updates_ApplicationType()
        {
            var model = new AccreditationBaseViewModel();
            var dto = new AccreditationDto { ApplicationTypeId = (int)ApplicationType.Exporter };

            model.Accreditation = dto;

            Assert.AreEqual(dto, model.Accreditation);
            Assert.AreEqual(ApplicationType.Exporter, model.ApplicationType);
            Assert.AreEqual("PERN", model.ApplicationTypeDescription);
        }

        [TestMethod]
        public void GetApplicationType_ValidId_ReturnsEnum()
        {
            var model = new AccreditationBaseViewModel();

            var result = model.GetApplicationType((int)ApplicationType.Reprocessor);

            Assert.AreEqual(ApplicationType.Reprocessor, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetApplicationType_InvalidId_Throws()
        {
            var model = new AccreditationBaseViewModel();

            model.GetApplicationType(999); // Not a valid ApplicationType
        }

        [TestMethod]
        public void ApplicationTypeDescription_Returns_PRN_Or_PERN()
        {
            var model = new AccreditationBaseViewModel();

            model.ApplicationType = ApplicationType.Reprocessor;
            Assert.AreEqual("PRN", model.ApplicationTypeDescription);

            model.ApplicationType = ApplicationType.Exporter;
            Assert.AreEqual("PERN", model.ApplicationTypeDescription);
        }
    }
}
