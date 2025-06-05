using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services;

[TestClass]
public class MaterialServiceTests : BaseServiceTests<MaterialService>
{
    private MaterialService _systemUnderTest = null!;

    [TestInitialize]
    public void Setup()
    {
        SetupEachTest();
        _systemUnderTest = new MaterialService(MockFacadeClient.Object, NullLogger);
    }

    [TestMethod]
    public async Task GetAllMaterials_DataReturned()
    {
        // Arrange
        var materials = new List<MaterialDto>
        {
            new() { Name = MaterialItem.Wood, Code = "W1" },
            new() { Name = MaterialItem.Aluminium, Code = "A1" }
        };
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(materials))
        };

        var expectedMaterials = new List<MaterialDto>
        {
            new() { Name = MaterialItem.Aluminium, Code = "A1" },
            new() { Name = MaterialItem.Wood, Code = "W1" }
        };

        // Expectations
        MockFacadeClient.Setup(o => o.SendGetRequest(Endpoints.Material.GetAllMaterials)).ReturnsAsync(response);

        // Act
        var result = await _systemUnderTest.GetAllMaterialsAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedMaterials);
    }

    [TestMethod]
    public async Task GetAllMaterials_OnlyReturnVisibleItems()
    {
        // Arrange
        var materials = new List<MaterialDto>
        {
            new() { Name = MaterialItem.Wood, Code = "W1" },
            new() { Name = MaterialItem.Aluminium, Code = "A1" },
            new() { Name = MaterialItem.None, Code = "N1" },
            new() { Name = MaterialItem.GlassRemelt, Code = "G1" }
        };

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(materials))
        };

        var expectedMaterials = new List<MaterialDto>
        {
            new() { Name = MaterialItem.Aluminium, Code = "A1" },
            new() { Name = MaterialItem.Wood, Code = "W1" }
        };

        // Expectations
        MockFacadeClient.Setup(o => o.SendGetRequest(Endpoints.Material.GetAllMaterials)).ReturnsAsync(response);

        // Act
        var result = await _systemUnderTest.GetAllMaterialsAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedMaterials);
    }

    [TestMethod]
    public async Task GetAllMaterials_NoData()
    {
        // Arrange
        var materials = new List<MaterialDto>();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(materials))
        };

        // Expectations
        MockFacadeClient.Setup(o => o.SendGetRequest(Endpoints.Material.GetAllMaterials)).ReturnsAsync(response);

        // Act
        var result = await _systemUnderTest.GetAllMaterialsAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [TestMethod]
    public async Task GetAllMaterials_NoContent()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.NoContent);
        
        // Expectations
        MockFacadeClient.Setup(o => o.SendGetRequest(Endpoints.Material.GetAllMaterials)).ReturnsAsync(response);

        // Act
        var result = await _systemUnderTest.GetAllMaterialsAsync();

        // Assert
        result.Should().BeEmpty();
    }
}