using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Extensions;

[TestClass]
public class MaterialItemExtensionsUnitTests
{
    [TestMethod]
    [DataRow(MaterialItem.None, "None")]
    [DataRow(MaterialItem.Plastic, "Plastic")]
    [DataRow(MaterialItem.Wood, "Wood")]
    [DataRow(MaterialItem.Aluminium, "Aluminium")]
    [DataRow(MaterialItem.Steel, "Steel")]
    [DataRow(MaterialItem.Paper, "Paper/Board")]
    [DataRow(MaterialItem.Glass, "Glass")]
    [DataRow(MaterialItem.GlassRemelt, "GlassRemelt")]
    [DataRow(MaterialItem.FibreComposite, "FibreComposite")]
    public void MaterialItemExtensions_EnsureAllCorrectValues(MaterialItem item, string expectedValue)
    {
        // Act
        var result = item.GetMaterialName();

        // Assert
        result.Should().BeEquivalentTo(expectedValue);
    }

    [TestMethod]
    [DataRow(MaterialItem.None, "None")]
    [DataRow(MaterialItem.Plastic, "Plastic")]
    [DataRow(MaterialItem.Wood, "Wood")]
    [DataRow(MaterialItem.Aluminium, "Aluminium")]
    [DataRow(MaterialItem.Steel, "Steel")]
    [DataRow(MaterialItem.Paper, "Paper/Board")]
    [DataRow(MaterialItem.Glass, "Glass")]
    [DataRow(MaterialItem.GlassRemelt, "GlassRemelt")]
    [DataRow(MaterialItem.FibreComposite, "FibreComposite")]
    public void MaterialItemExtensions_EnsureCanParse(MaterialItem expectedItem, string input)
    {
        // Act
        var result = MaterialItemExtensions.GetMaterialName(input);

        // Assert
        result.Should().Be(expectedItem);
    }
}