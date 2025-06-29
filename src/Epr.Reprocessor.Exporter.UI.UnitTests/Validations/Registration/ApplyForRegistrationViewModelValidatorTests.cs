using Epr.Reprocessor.Exporter.UI.Validations.Registration;

namespace Epr.Reprocessor.Exporter.Tests.Validations.Registration;

[TestClass]
public class ApplyForRegistrationViewModelValidatorTests
{
    private ApplyForRegistrationViewModelValidator _validator;

    [TestInitialize]
    public void Setup()
    {
        _validator = new ApplyForRegistrationViewModelValidator();
    }

    [TestMethod]
    [DataRow(ApplicationType.Exporter)]
    [DataRow(ApplicationType.Reprocessor)]
    public void Should_Validate_When_ApplicationType_Is_Valid(ApplicationType applicationType)
    {
        // Arrange
        var viewModel = new ApplyForRegistrationViewModel
        {
            ApplicationType = applicationType
        };

        // Act
        var result = _validator.Validate(viewModel);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public void Should_Not_Validate_When_ApplicationType_Is_Unspecified()
    {
        // Arrange
        var viewModel = new ApplyForRegistrationViewModel
        {
            ApplicationType = ApplicationType.Unspecified
        };

        // Act
        var result = _validator.Validate(viewModel);

        // Assert
        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle();
        }
    }

    [TestMethod]
    public void Should_Not_Validate_When_ApplicationType_Is_Invalid()
    {
        // Arrange
        var invalidApplicationType = (ApplicationType)999; // Invalid enum value, out of the defined enum values

        var viewModel = new ApplyForRegistrationViewModel
        {
            ApplicationType = invalidApplicationType
        };

        // Act
        var result = _validator.Validate(viewModel);

        // Assert
        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle();
        }
    }
}

