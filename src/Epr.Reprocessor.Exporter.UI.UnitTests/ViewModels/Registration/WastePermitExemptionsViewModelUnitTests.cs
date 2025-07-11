namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels.Registration;

[TestClass]
public class WastePermitExemptionsViewModelUnitTests
{
    [TestMethod]
    public void WastePermitExemptionsViewModel_SelectedMaterialsEmpty_ReturnFalse()
    {
        // Arrange
        var model = new WastePermitExemptionsViewModel
        {
            SelectedMaterials = new List<string>()
        };
        var validationResults = new List<ValidationResult>();

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true);

        // Assert
        result.Should().BeFalse();
        validationResults.Should().BeEquivalentTo(new List<ValidationResult>
        {
            new(
                "Select all the material categories the site has a permit or exemption to accept and recycle",
                new List<string> { "SelectedMaterials" })
        });
    }

    [TestMethod]
    public void WastePermitExemptionsViewModel_SelectedMaterialsNotEmpty_ReturnTrue()
    {
        // Arrange
        var model = new WastePermitExemptionsViewModel
        {
            SelectedMaterials = ["steel"]
        };
        var validationResults = new List<ValidationResult>();

        // Act
        var result = Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true);

        // Assert
        result.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }
}
