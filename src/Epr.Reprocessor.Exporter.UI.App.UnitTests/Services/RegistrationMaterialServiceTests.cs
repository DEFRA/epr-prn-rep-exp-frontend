﻿using System.Text.Json;
using Epr.Reprocessor.Exporter.UI.App.Enums.Registration;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services;

[TestClass]
public class RegistrationMaterialServiceTests : BaseServiceTests<RegistrationMaterialService>
{
    private RegistrationMaterialService _systemUnderTest = null!;
    private JsonSerializerOptions _serializerOptions = null!;

    [TestInitialize]
    public void Setup()
    {
        SetupEachTest();
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

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
        await Assert.ThrowsExactlyAsync<Exception>(async () =>
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
    public async Task DeleteAsync_SuccessfulRequest_CallsApiClientWithCorrectParameters()
    {
        // Arrange
        var registrationMaterialId = Guid.NewGuid();
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
        };

        // Expectations
        MockFacadeClient
            .Setup(x => x.SendDeleteRequest(string.Format(Endpoints.RegistrationMaterial.Delete, registrationMaterialId)))
            .ReturnsAsync(response);

        // Act
        var result = _systemUnderTest.DeleteAsync(registrationMaterialId);
        await result;

        // Assert
        result.Should().BeEquivalentTo(Task.CompletedTask);
    }

    [TestMethod]
    public async Task UpsertRegistrationReprocessingDetailsAsync_SuccessfulRequest_CallsApiClientWithCorrectParameters()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var dto = new RegistrationReprocessingIODto
        {
            TypeOfSuppliers = "Supplier 123",
        };

        MockFacadeClient
            .Setup(x => x.SendPostRequest(string.Format(Endpoints.RegistrationMaterial.UpsertRegistrationReprocessingDetails, id), dto));

        // Act
        await _systemUnderTest.UpsertRegistrationReprocessingDetailsAsync(id, dto);

        // Assert
        MockFacadeClient.Verify(x => x.SendPostRequest(string.Format(Endpoints.RegistrationMaterial.UpsertRegistrationReprocessingDetails, id), dto), Times.Once);
    }

    [TestMethod]
    public async Task UpsertRegistrationReprocessingDetailsAsync_ApiClientReturnsError_ThrowsException()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var dto = new RegistrationReprocessingIODto
        {
            TypeOfSuppliers = "Supplier 123",
        };

        MockFacadeClient
            .Setup(x => x.SendPostRequest(string.Format(Endpoints.RegistrationMaterial.UpsertRegistrationReprocessingDetails, id), dto))
            .Throws(new Exception());

        // Act & Assert
        await Assert.ThrowsExactlyAsync<Exception>(async () =>
        {
            await _systemUnderTest.UpsertRegistrationReprocessingDetailsAsync(id, dto);
        });
    }
}