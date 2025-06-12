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
    public async Task CreateRegistrationMaterialAndExemptionReferences_SuccessfulRequest_CallsApiClientWithCorrectParameters()
    {
        // Arrange
        var dto = new CreateRegistrationMaterialAndExemptionReferencesDto
        {
            RegistrationMaterial = new RegistrationMaterialDto
            {
                MaterialName = "Test Material"
            },
            MaterialExemptionReferences = new List<MaterialExemptionReferenceDto>()
        };

        MockFacadeClient
            .Setup(x => x.SendPostRequest(Endpoints.RegistrationMaterial.CreateRegistrationMaterialAndExemptionReferences, dto));            

        // Act
        await _systemUnderTest.CreateRegistrationMaterialAndExemptionReferences(dto);

        // Assert
        MockFacadeClient.Verify(x => x.SendPostRequest(Endpoints.RegistrationMaterial.CreateRegistrationMaterialAndExemptionReferences, dto), Times.Once);
    }

    [TestMethod]
    public async Task CreateRegistrationMaterialAndExemptionReferences_ApiClientReturnsError_ThrowsException()
    {
        // Arrange
        var dto = new CreateRegistrationMaterialAndExemptionReferencesDto
        {
            RegistrationMaterial = new RegistrationMaterialDto
            {
                MaterialName = "Test Material"
            },
            MaterialExemptionReferences = new List<MaterialExemptionReferenceDto>()
        };

        MockFacadeClient
            .Setup(x => x.SendPostRequest(Endpoints.RegistrationMaterial.CreateRegistrationMaterialAndExemptionReferences, dto))
            .Throws(new Exception());

        // Act & Assert
        await Assert.ThrowsExceptionAsync<Exception>(async () =>
        {
            await _systemUnderTest.CreateRegistrationMaterialAndExemptionReferences(dto);
        });
    }
}
