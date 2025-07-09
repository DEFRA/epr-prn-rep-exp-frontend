namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels.Registration;

[TestClass]
public class ExemptionReferencesViewModelUnitTests
{
    [TestMethod]
    public void ExemptionReferencesViewModel_ValidModel_OneFieldPopulated()
    {
        // Arrange
        var model = new ExemptionReferencesViewModel
        {
            ExemptionReferences1 = "value"
        };

        var validationResults = new List<ValidationResult>();

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults);

        // Assert
        result.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }

    [TestMethod]
    public void ExemptionReferencesViewModel_ValidModel_AllFieldsPopulated()
    {
        // Arrange
        var model = new ExemptionReferencesViewModel
        {
            ExemptionReferences1 = "value1",
            ExemptionReferences2 = "value2",
            ExemptionReferences3 = "value3",
            ExemptionReferences4 = "value4",
            ExemptionReferences5 = "value5"
        };

        var validationResults = new List<ValidationResult>();

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults);

        // Assert
        result.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }

    [TestMethod]
    public void ExemptionReferencesViewModel_InvalidModel_NoFieldsPopulated_EnsureValidationResultsPopulated()
    {
        // Arrange
        var model = new ExemptionReferencesViewModel();

        var validationResults = new List<ValidationResult>();
        var expectedResults = new List<ValidationResult> { new("Enter at least one exemption reference", new List<string>{nameof(model.ExemptionReferences1)}) };

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults);

        // Assert
        result.Should().BeFalse();
        validationResults.Should().BeEquivalentTo(expectedResults);
    }

    [TestMethod]
    public void ExemptionReferencesViewModel_InvalidModel_AllFieldsPopulated_OneDuplicatedEntry()
    {
        // Arrange
        var model = new ExemptionReferencesViewModel
        {
            ExemptionReferences1 = "duplicate",
            ExemptionReferences2 = "duplicate",
            ExemptionReferences3 = "value3",
            ExemptionReferences4 = "value4",
            ExemptionReferences5 = "value5"
        };

        var validationResults = new List<ValidationResult>();
        var expectedResults = new List<ValidationResult>
        {
            new("Exemption reference number already added", new List<string> { nameof(model.ExemptionReferences2) })
        };

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults);

        // Assert
        result.Should().BeFalse();
        validationResults.Should().BeEquivalentTo(expectedResults);
    }

    [TestMethod]
    public void ExemptionReferencesViewModel_InvalidModel_AllFieldsPopulated_TwoDuplicatedEntry()
    {
        // Arrange
        var model = new ExemptionReferencesViewModel
        {
            ExemptionReferences1 = "duplicate",
            ExemptionReferences2 = "duplicate",
            ExemptionReferences3 = "duplicate",
            ExemptionReferences4 = "value4",
            ExemptionReferences5 = "value5"
        };

        var validationResults = new List<ValidationResult>();
        var expectedResults = new List<ValidationResult>
        {
            new("Exemption reference number already added", new List<string> { nameof(model.ExemptionReferences2), nameof(model.ExemptionReferences3) })
        };

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults);

        // Assert
        result.Should().BeFalse();
        validationResults.Should().BeEquivalentTo(expectedResults);
    }

    [TestMethod]
    public void ExemptionReferencesViewModel_InvalidModel_AllFieldsPopulated_TwoDistinctDuplicatedEntry()
    {
        // Arrange
        var model = new ExemptionReferencesViewModel
        {
            ExemptionReferences1 = "duplicate",
            ExemptionReferences2 = "duplicate",
            ExemptionReferences3 = "duplicate1",
            ExemptionReferences4 = "duplicate1",
            ExemptionReferences5 = "value5"
        };

        var validationResults = new List<ValidationResult>();
        var expectedResults = new List<ValidationResult>
        {
            new("Exemption reference number already added", new List<string> { nameof(model.ExemptionReferences2) }),
            new("Exemption reference number already added", new List<string> { nameof(model.ExemptionReferences4) })
        };

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults);

        // Assert
        result.Should().BeFalse();
        validationResults.Should().BeEquivalentTo(expectedResults);
    }
}

[TestClass]
public class CheckYourAnswersWasteDetailsViewModelUnitTests
{
    [TestMethod]
    public void METHOD()
    {
        // Arrange
        var sut = new CheckYourAnswersWasteDetailsViewModel();

        // Act
        sut.SetPackagingWasteDetails(new PackagingWaste
        {
            SelectedMaterials =
            [
                new()
                {
                    Id = Guid.Empty,
                    Name = Material.Aluminium,
                    PermitType = PermitType.InstallationPermit,
                    PermitNumber = "123",
                    WeightInTonnes = 10,
                    PermitPeriod = PermitPeriod.PerMonth,
                    MaxCapableWeightInTonnes = 20,
                    MaxCapableWeightPeriodDuration = PeriodDuration.PerYear
                },

                //new()
                //{
                //    Id = Guid.Empty,
                //    Name = Material.Glass,
                //    PermitType = PermitType.PollutionPreventionAndControlPermit,
                //    PermitNumber = "456",
                //    WeightInTonnes = 20,
                //    PermitPeriod = PermitPeriod.PerWeek,
                //    MaxCapableWeightInTonnes = 30,
                //    MaxCapableWeightPeriodDuration = PeriodDuration.PerMonth
                //},

                //new()
                //{
                //    Id = Guid.Empty,
                //    Name = Material.Plastic,
                //    PermitType = PermitType.WasteExemption,
                //    MaxCapableWeightInTonnes = 30,
                //    MaxCapableWeightPeriodDuration = PeriodDuration.PerMonth,
                //    Exemptions = new List<Exemption>
                //    {
                //        new() { ReferenceNumber = "reference1" },
                //        new() { ReferenceNumber = "reference2" }
                //    }
                //}
            ]
        });

        // Assert
        sut.PackagingWasteDetailsSummaryList.Should().BeEquivalentTo(new SummaryListModel
        {
            Rows =
            [
                new()
                {
                    Key = "Packaging waste the site has a permit or exemption to accept and recycle",
                    Value = "Aluminium",
                    ChangeLinkUrl = PagePaths.WastePermitExemptions,
                    ChangeLinkHiddenAccessibleText = "the selected materials"
                }
            ]
        });

        sut.MaterialPermits.Should().BeEquivalentTo(new List<SummaryListModel>
        {
            new ()
            {
                Heading = "Permit for accepting and reprocessing Aluminium",
                Rows =
                [
                    new()
                    {
                        Key = "Installation permit",
                        Value = "123",
                        ChangeLinkUrl = PagePaths.PermitForRecycleWaste,
                        ChangeLinkHiddenAccessibleText = "the permit details"
                    }
                ]
            }
        });
    }
}