using Epr.Reprocessor.Exporter.UI.App.Services;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services;

[TestClass]
public class ValidationServiceTests
{
    private Mock<IServiceProvider> _serviceProviderMock;
    private ValidationService _validationService;

    [TestInitialize]
    public void Setup()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _validationService = new ValidationService(_serviceProviderMock.Object);
    }

    public class TestModel
    {
        public string Name { get; set; } = string.Empty;
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldReturnValidationResult_WhenValidatorExists()
    {
        // Arrange
        var testModel = new TestModel { Name = "Test" };
        var expectedResult = new ValidationResult();

        var validatorMock = new Mock<IValidator<TestModel>>();
        validatorMock.Setup(v => v.ValidateAsync(testModel, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(expectedResult);

        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IValidator<TestModel>)))
                             .Returns(validatorMock.Object);

        // Act
        var result = await _validationService.ValidateAsync(testModel);

        // Assert
        
        result.Should().NotBeNull();
        result.Should().Be(expectedResult);
        validatorMock.Verify(v => v.ValidateAsync(testModel, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldThrowInvalidOperationException_WhenValidatorIsNotRegistered()
    {
        // Arrange
        var testModel = new TestModel { Name = "Test" };

        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IValidator<TestModel>)))
                             .Returns(null); // No validator registered

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
        {
            await _validationService.ValidateAsync(testModel);
        });

        exception.Message.Should().Be("No validator found for type TestModel");
    }
}
