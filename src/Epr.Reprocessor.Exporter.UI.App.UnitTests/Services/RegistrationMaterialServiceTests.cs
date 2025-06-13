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
}
