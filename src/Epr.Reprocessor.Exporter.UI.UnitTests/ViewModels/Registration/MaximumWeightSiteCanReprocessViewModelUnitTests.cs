namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels.Registration;

[TestClass]
public class MaximumWeightSiteCanReprocessViewModelUnitTests
{
    private static MaximumWeightSiteCanReprocessViewModel GetValidViewModel()
    {
        return new()
        {
            Material = "steel",
            SelectedFrequency = PermitPeriod.PerMonth,
            MaximumWeight = "1000",
            MaterialType = MaterialType.Permit
        };
    }

    [TestMethod]
    public void Constructor()
    {
        // Arrange
        var sut = new MaximumWeightSiteCanReprocessViewModel();

        // Assert
        sut.Should().NotBeNull();
    }

    [TestMethod]
    public void MaximumWeightSiteCanReprocessViewModel_PropertyAssignment_Works()
    {
        // Arrange
        var model = GetValidViewModel();

        // Act - properties were already set during instantiation

        // Assert
        model.Should().BeEquivalentTo(new MaximumWeightSiteCanReprocessViewModel
        {
            Material = "steel",
            SelectedFrequency = PermitPeriod.PerMonth,
            MaximumWeight = "1000",
            MaterialType = MaterialType.Permit
        });
    }

    [TestMethod]
    public void MaximumWeightSiteCanReprocessViewModel_WithValidData_PassesValidation()
    {
        // Arrange
        var model = GetValidViewModel();
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [TestMethod]
    public void MaximumWeightSiteCanReprocessViewModel_MissingRequiredFields_FailsValidation()
    {
        // Arrange
        var model = new MaximumWeightSiteCanReprocessViewModel(); // leave properties unset
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        isValid.Should().BeFalse();

        results.Should().BeEquivalentTo(new List<ValidationResult>
        {
            new("Select if the weight that can be reprocessed is per year, per month or per week", new List<string>{"SelectedFrequency"}),
            new("Enter the maximum weight the site could reprocess", new List<string>{"MaximumWeight"}),
        });
    }

    [TestMethod]
    public void MaximumWeightSiteCanReprocessViewModel_PopulatedFrequencyAndWeight_ButWeightFailsTonnageValidation()
    {
        // Arrange
        var model = GetValidViewModel();
        model.MaximumWeight = "invalid"; // Set an invalid weight to trigger TonnageValidation failure
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        isValid.Should().BeFalse();

        results.Should().BeEquivalentTo(new List<ValidationResult>
        {
            new("Weight must be a number, like 100", new List<string>{"MaximumWeight"}),
        });
    }
}