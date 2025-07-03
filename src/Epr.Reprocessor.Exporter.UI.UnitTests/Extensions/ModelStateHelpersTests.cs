namespace Epr.Reprocessor.Exporter.UI.UnitTests.Extensions;

[TestClass]
public class ModelStateHelpersTests
{
    private ModelStateDictionary _modelStateDictionary;

    [TestInitialize]
    public void SetUp()
    {
        _modelStateDictionary = new ModelStateDictionary();
    }

    [TestMethod]
    public void AddExceptionsToModel_AddsErrorMessageToMode_WhenCalled()
    {
        // Arrange
        var exceptionCodes = new List<string> { "81" };

        // Act
        ModelStateHelpers.AddFileUploadExceptionsToModelState(exceptionCodes, _modelStateDictionary);

        // Assert
        var modelStateErrors = GetModelStateErrors();
        modelStateErrors.Should().NotBeNullOrEmpty();
        modelStateErrors.Count().Should().Be(1);
        modelStateErrors.Contains("The selected file contains a virus");
    }

    private IEnumerable<string> GetModelStateErrors()
    {
        return _modelStateDictionary.Values
            .SelectMany(x => x.Errors)
            .Select(x => x.ErrorMessage);
    }
}
