using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.App.Extensions;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Extensions;

[TestClass]
public class MaterialExtensionsUnitTests
{
    [TestMethod]
    [DataRow(Material.Aluminium)]
    [DataRow(Material.FibreComposite)]
    [DataRow(Material.Glass)]
    [DataRow(Material.Paper, "Paper/Board")]
    [DataRow(Material.GlassRemelt)]
    [DataRow(Material.Plastic)]
    [DataRow(Material.Steel)]
    [DataRow(Material.Wood)]
    public void GetMaterialName_EnsureCorrectMaterialNamesReturned(Material material, string? expected = null)
    {
        // Arrange
        
        // Act
        var result = material.GetMaterialName();

        // Assert
        result.Should().BeEquivalentTo(string.IsNullOrEmpty(expected) ? material.ToString() : expected);
    }
}