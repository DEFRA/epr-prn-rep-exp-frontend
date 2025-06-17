using System.Text.Json;
using Epr.Reprocessor.Exporter.UI.App.DTOs;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services;

[TestClass]
public class RegistrationMaterialServiceTests : BaseServiceTests<RegistrationMaterialService>
{
    private RegistrationMaterialService _systemUnderTest = null!;

    [TestInitialize]
    public void Setup()
    {
        SetupEachTest();
        _systemUnderTest = new RegistrationMaterialService(MockFacadeClient.Object, NullLogger);
    }

    [TestMethod]
    public async Task CreateExemptionReferences_SuccessfulRequest_CallsApiClientWithCorrectParameters()
    {
        // Arrange
        var dto = new CreateExemptionReferencesDto
        {
            MaterialExemptionReferences = new List<MaterialExemptionReferenceDto>()
        };

        MockFacadeClient
            .Setup(x => x.SendPostRequest(Endpoints.MaterialExemptionReference.CreateMaterialExemptionReferences, dto));

        // Act
        await _systemUnderTest.CreateExemptionReferences(dto);

        // Assert
        MockFacadeClient.Verify(x => x.SendPostRequest(Endpoints.MaterialExemptionReference.CreateMaterialExemptionReferences, dto), Times.Once);
    }

    [TestMethod]
    public async Task CreateExemptionReferences_ApiClientReturnsError_ThrowsException()
    {
        // Arrange
        var dto = new CreateExemptionReferencesDto
        {
            MaterialExemptionReferences = new List<MaterialExemptionReferenceDto>()
        };

        MockFacadeClient
            .Setup(x => x.SendPostRequest(Endpoints.MaterialExemptionReference.CreateMaterialExemptionReferences, dto))
            .Throws(new Exception());

        // Act & Assert
        await Assert.ThrowsExceptionAsync<Exception>(async () =>
        {
            await _systemUnderTest.CreateExemptionReferences(dto);
        });
    }

    [TestMethod]
    public async Task GetAllRegistrationMaterialsAsync_SuccessfulRequest_CallsApiClientWithCorrectParameters()
    {
        // Arrange
        var registrationId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var registrationMaterialsDto = new List<RegistrationMaterialDto>
        {
            new()
            {
                Id = id,
                RegistrationId = registrationId
            }
        };

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(registrationMaterialsDto, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }))
        };

        // Expectations
        MockFacadeClient
            .Setup(x => x.SendGetRequest(string.Format(Endpoints.RegistrationMaterial.GetAllRegistrationMaterials, registrationId)))
            .ReturnsAsync(response);

        // Act
        var result = await _systemUnderTest.GetAllRegistrationMaterialsAsync(registrationId);

        // Assert
        result.Should().BeEquivalentTo(registrationMaterialsDto);
    }
}