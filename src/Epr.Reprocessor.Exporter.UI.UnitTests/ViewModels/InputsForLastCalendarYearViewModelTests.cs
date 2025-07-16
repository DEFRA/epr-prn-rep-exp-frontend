namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels;

[TestClass]
public class InputsForLastCalendarYearViewModelTests
{
    [TestMethod]
    [DataRow("1,000", null, null, 1000)]
    [DataRow(null, "1,000", null, 1000)]
    [DataRow(null, null, "1,000", 1000)]
    [DataRow("1,000", "1,000", "1,000", 3000)]
    public void TotalInputTonnes_ShouldReturnCorrectValue(string? ukPackagingWaste, string? nonUkPackagingWaste, string? nonPackagingWaste, int? expectedTotalInputTonnes)
    {
        // Arrange
        var viewModel = new InputsForLastCalendarYearViewModel
        {
            UkPackagingWaste = ukPackagingWaste,
            NonUkPackagingWaste = nonUkPackagingWaste,
            NonPackagingWaste = nonPackagingWaste
        };

        // Act
        var totalInputTonnes = viewModel.TotalInputTonnes;

        // Assert
        totalInputTonnes.Should().Be(expectedTotalInputTonnes);
    }

    [TestMethod]
    [DataRow("1,000", null, null, "1,000|2,000", 4000)] 
    [DataRow(null, "1,000", null, "3,000", 4000)]       
    [DataRow(null, null, "1,000", "abc|5,000", 6000)]   
    [DataRow("1,000", "1,000", "1,000", "1,000|1,000|1,000", 6000)] 
    [DataRow("1000", "2,000", "3000", "1000|2,000", 9000)] 
    public void TotalInputTonnes_WithWasteAndRawMaterials_ShouldReturnCorrectValue(
     string? ukPackagingWaste,
     string? nonUkPackagingWaste,
     string? nonPackagingWaste,
     string rawMaterialTonnesPipeSeparated,
     int expectedTotalInputTonnes)
    {
        // Arrange
        var rawMaterials = rawMaterialTonnesPipeSeparated
            .Split('|', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => new RawMaterialRowViewModel
            {
                RawMaterialName = "Material",
                Tonnes = t
            })
            .ToList();

        var viewModel = new InputsForLastCalendarYearViewModel
        {
            UkPackagingWaste = ukPackagingWaste,
            NonUkPackagingWaste = nonUkPackagingWaste,
            NonPackagingWaste = nonPackagingWaste,
            RawMaterials = rawMaterials
        };

        // Act
        var totalInputTonnes = viewModel.TotalInputTonnes;

        // Assert
        totalInputTonnes.Should().Be(expectedTotalInputTonnes);
    }
}
