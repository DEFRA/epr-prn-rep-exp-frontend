using System.Text.Json;
using Epr.Reprocessor.Exporter.UI.App.Enums.Registration;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services;

[TestClass]
public class RegistrationMaterialServiceTests : BaseServiceTests<RegistrationMaterialService>
{
    private RegistrationMaterialService _systemUnderTest = null!;
    private JsonSerializerOptions _serializerOptions = null!;
    private Mock<ILogger<RegistrationMaterialService>> _loggerMock;

    [TestInitialize]
    public void Setup()
    {
        SetupEachTest();
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        _loggerMock = new Mock<ILogger<RegistrationMaterialService>>();

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
            Content = new StringContent(JsonSerializer.Serialize(registrationMaterialsDto, _serializerOptions))
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

    [TestMethod]
    public async Task UpdateRegistrationMaterialPermitsAsync_SuccessfulRequest_CallsApiClientWithCorrectParameters()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var dto = new UpdateRegistrationMaterialPermitsDto
        {
            PermitNumber = "TEST123",
            PermitTypeId = 2
        };

        MockFacadeClient
            .Setup(x => x.SendPostRequest(string.Format(Endpoints.RegistrationMaterial.UpdateRegistrationMaterialPermits, id), dto));

        // Act
        await _systemUnderTest.UpdateRegistrationMaterialPermitsAsync(id, dto);

        // Assert
        MockFacadeClient.Verify(x => x.SendPostRequest(string.Format(Endpoints.RegistrationMaterial.UpdateRegistrationMaterialPermits, id), dto), Times.Once);
    }

    [TestMethod]
    public async Task UpdateRegistrationMaterialPermitsAsync_ApiClientReturnsError_ThrowsException()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var dto = new UpdateRegistrationMaterialPermitsDto
        {
            PermitNumber = "TEST123",
            PermitTypeId = 2
        };

        MockFacadeClient
            .Setup(x => x.SendPostRequest(string.Format(Endpoints.RegistrationMaterial.UpdateRegistrationMaterialPermits, id), dto))
            .Throws(new Exception());

        // Act & Assert
        await Assert.ThrowsExactlyAsync<Exception>(async () =>
        {
            await _systemUnderTest.UpdateRegistrationMaterialPermitsAsync(id, dto);
        });
    }


    [TestMethod]
    public async Task GetMaterialsPermitTypesAsync_SuccessfulRequest_CallsApiClientWithCorrectParameters()
    {
        // Arrange
        var materialPermitTypes = Enum.GetValues(typeof(MaterialPermitType))
                   .Cast<MaterialPermitType>()
                   .Select(e => new MaterialsPermitTypeDto
                   {
                       Id = (int)e,
                       Name = e.ToString()
                   })
                   .Where(x => x.Id > 0)
                   .ToList();

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(materialPermitTypes, _serializerOptions))
        };

        // Expectations
        MockFacadeClient
            .Setup(x => x.SendGetRequest(Endpoints.RegistrationMaterial.GetMaterialsPermitTypes))
            .ReturnsAsync(response);

        // Act
        var result = await _systemUnderTest.GetMaterialsPermitTypesAsync();

        // Assert
        result.Should().BeEquivalentTo(materialPermitTypes);
    }

    [TestMethod]
    public async Task UpdateRegistrationMaterialPermitCapacityAsync_Success_CallsClientWithCorrectParameters()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateRegistrationMaterialPermitCapacityDto
        {
            PermitTypeId = 1,
            CapacityInTonnes = 10.5m,
            PeriodId = 2
        };
        var expectedUri = string.Format(Endpoints.RegistrationMaterial.UpdateRegistrationMaterialPermitCapacity, id);

        MockFacadeClient
            .Setup(c => c.SendPostRequest(expectedUri, request))
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)))
            .Verifiable();

        // Act
        await _systemUnderTest.UpdateRegistrationMaterialPermitCapacityAsync(id, request);

        // Assert
        MockFacadeClient.Verify(c => c.SendPostRequest(expectedUri, request), Times.Once);
    }

    [TestMethod]
    public async Task UpdateRegistrationMaterialPermitCapacityAsync_HttpRequestException_LogsErrorAndThrows()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateRegistrationMaterialPermitCapacityDto
        {
            PermitTypeId = 1,
            CapacityInTonnes = 10.5m,
            PeriodId = 2
        };
        var expectedUri = string.Format(Endpoints.RegistrationMaterial.UpdateRegistrationMaterialPermitCapacity, id);
        var exception = new HttpRequestException($"Failed to update registration material for registration permit capacity with External ID {id}");

        // Use the logger mock in the SUT
        _systemUnderTest = new RegistrationMaterialService(MockFacadeClient.Object, _loggerMock.Object);

        MockFacadeClient
            .Setup(c => c.SendPostRequest(expectedUri, request))
            .ThrowsAsync(exception);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<HttpRequestException>(() =>
            _systemUnderTest.UpdateRegistrationMaterialPermitCapacityAsync(id, request));

        Assert.AreEqual(exception, ex);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to update registration material for registration permit capacity with External ID")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once
        );
    }
}