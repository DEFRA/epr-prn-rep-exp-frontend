using Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels.Accreditation
{
    [TestClass]
    public class BusinessPlanViewModelTests
    {
        [TestMethod]
        public void Validate_WhenNoValuesProvided_ReturnsErrorsWithAllFieldsAndTotal()
        {
            var model = new BusinessPlanViewModel
            {
                Subject = "MaterialX"
            };

            var context = new ValidationContext(model);
            var results = model.Validate(context).ToList();

            Assert.AreEqual(2, results.Count);

            Assert.IsTrue(results.Any(r =>
                r.MemberNames.Contains(nameof(BusinessPlanViewModel.TotalEntered))));

            Assert.IsTrue(results.Any(r =>
                r.MemberNames.Contains(nameof(BusinessPlanViewModel.InfrastructurePercentage))));
        }

        [TestMethod]
        public void Validate_WhenTotalDoesNotEqual100_ReturnsTotalMustBe100Error()
        {
            var model = new BusinessPlanViewModel
            {
                Subject = "MaterialY",
                InfrastructurePercentage = "40",
                PackagingWastePercentage = "40",
                BusinessCollectionsPercentage = "10"
            };

            var context = new ValidationContext(model);
            var results = model.Validate(context).ToList();

            Assert.IsTrue(results.Any(r =>
                r.MemberNames.Contains(nameof(BusinessPlanViewModel.TotalEntered))));
        }

        [TestMethod]
        public void Validate_WhenNegativeValue_ReturnsErrorForInfrastructure()
        {
            var model = new BusinessPlanViewModel
            {
                Subject = "MaterialZ",
                InfrastructurePercentage = "-10"
            };

            var context = new ValidationContext(model);
            var results = model.Validate(context).ToList();

            Assert.IsTrue(results.Count >= 1);
            Assert.IsTrue(results.Any(r =>
                r.MemberNames.Contains(nameof(BusinessPlanViewModel.InfrastructurePercentage))));
        }

        [TestMethod]
        public void Validate_WhenValueExceeds100_ReturnsErrorForPackagingWaste()
        {
            var model = new BusinessPlanViewModel
            {
                Subject = "MaterialQ",
                PackagingWastePercentage = "101"
            };

            var context = new ValidationContext(model);
            var results = model.Validate(context).ToList();

            Assert.IsTrue(results.Count >= 1);
            Assert.IsTrue(results.Any(r =>
                r.MemberNames.Contains(nameof(BusinessPlanViewModel.PackagingWastePercentage))));
        }

        [TestMethod]
        public void Validate_WhenValueIsNotWholeNumber_ReturnsErrorForCommunications()
        {
            var model = new BusinessPlanViewModel
            {
                Subject = "MaterialW",
                CommunicationsPercentage = "50.5"
            };

            var context = new ValidationContext(model);
            var results = model.Validate(context).ToList();

            Assert.IsTrue(results.Count >= 1);
            Assert.IsTrue(results.Any(r =>
                r.MemberNames.Contains(nameof(BusinessPlanViewModel.CommunicationsPercentage))));
        }

        [TestMethod]
        public void Validate_WhenValueContainsNonDigits_ReturnsErrorForNewMarkets()
        {
            var model = new BusinessPlanViewModel
            {
                Subject = "MaterialNonDigit",
                NewMarketsPercentage = "abc"
            };

            var context = new ValidationContext(model);
            var results = model.Validate(context).ToList();

            Assert.IsTrue(results.Count >= 1);
            Assert.IsTrue(results.Any(r =>
                r.MemberNames.Contains(nameof(BusinessPlanViewModel.NewMarketsPercentage))));
        }

        [TestMethod]
        public void Validate_WhenAllValuesCorrectAndTotal100_ReturnsNoErrors()
        {
            var model = new BusinessPlanViewModel
            {
                Subject = "MaterialOk",
                InfrastructurePercentage = "25",
                PackagingWastePercentage = "25",
                BusinessCollectionsPercentage = "25",
                CommunicationsPercentage = "25"
            };

            var context = new ValidationContext(model);
            var results = model.Validate(context).ToList();

            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void Validate_WhenMultipleInvalidFields_ReturnsErrorsPerFieldAndTotalError()
        {
            var model = new BusinessPlanViewModel
            {
                Subject = "MaterialMultiple",
                InfrastructurePercentage = "-10",
                PackagingWastePercentage = "101",
                CommunicationsPercentage = "50.5",
                NewUsesPercentage = "abc"
            };

            var context = new ValidationContext(model);
            var results = model.Validate(context).ToList();

            Assert.IsTrue(results.Count >= 5);

            Assert.IsTrue(results.Any(r =>
                r.MemberNames.Contains(nameof(BusinessPlanViewModel.InfrastructurePercentage))));

            Assert.IsTrue(results.Any(r =>
                r.MemberNames.Contains(nameof(BusinessPlanViewModel.PackagingWastePercentage))));

            Assert.IsTrue(results.Any(r =>
                r.MemberNames.Contains(nameof(BusinessPlanViewModel.CommunicationsPercentage))));

            Assert.IsTrue(results.Any(r =>
                r.MemberNames.Contains(nameof(BusinessPlanViewModel.NewUsesPercentage))));

            Assert.IsTrue(results.Any(r =>
                r.MemberNames.Contains(nameof(BusinessPlanViewModel.TotalEntered))));
        }
    }
}
