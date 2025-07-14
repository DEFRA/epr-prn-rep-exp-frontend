namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels.Registration;

[TestClass]
public class MaterialPermitViewModelUnitTests
{
    private static MaterialPermitViewModel GetValidViewModel()
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
        var sut = new MaterialPermitViewModel();

        // Assert
        sut.Should().NotBeNull();
    }

    [TestMethod]
    public void MaterialPermitViewModel_PropertyAssignment_Works()
    {
        // Arrange
        var model = GetValidViewModel();

        // Act - properties were already set during instantiation

        // Assert
        model.Should().BeEquivalentTo(new MaterialPermitViewModel
        {
            Material = "steel",
            SelectedFrequency = PermitPeriod.PerMonth,
            MaximumWeight = "1000",
            MaterialType = MaterialType.Permit
        });
    }

    [TestMethod]
    public void MaterialPermitViewModel_WithValidData_PassesValidation()
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
    public void MaterialPermitViewModel_MissingRequiredFields_FailsValidation()
    {
        // Arrange
        var model = new MaterialPermitViewModel(); // leave properties unset
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        isValid.Should().BeFalse();

        results.Should().BeEquivalentTo(new List<ValidationResult>
        {
            new("Select if the authorised weight is per year, per month or per week", new List<string>{"SelectedFrequency"}),
            new("Enter the maximum weight the permit authorises the site to accept and recycle", new List<string>{"MaximumWeight"}),
        });
    }

    [TestMethod]
    public void MaterialPermitViewModel_PopulatedFrequencyAndWeight_ButWeightFailsTonnageValidation()
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