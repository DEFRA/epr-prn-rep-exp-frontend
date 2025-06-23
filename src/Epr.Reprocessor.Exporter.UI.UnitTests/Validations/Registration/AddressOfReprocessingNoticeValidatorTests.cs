using AutoFixture;
using Epr.Reprocessor.Exporter.UI.App.Domain;
using Epr.Reprocessor.Exporter.UI.Validations.Registration;
using FluentValidation.TestHelper;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Validations.Registration;

[TestClass]
public class AddressOfReprocessingNoticeValidatorTests
{
    private AddressOfReprocessingNoticeValidator _validator;
    private Fixture _fixture;

    [TestInitialize]
    public void Setup()
    {
        _validator = new AddressOfReprocessingNoticeValidator();

        _fixture = new Fixture();
    }

    [TestMethod]
    public void ShouldNotHaveError_When_ValidDataProvided()
    {
        // Arrange
        var model = _fixture.Build<AddressForNoticesViewModel>()
                .With(x => x.SelectedAddressOptions, AddressOptions.RegisteredAddress)
                .Create();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestMethod]
    public void ShouldHaveError_When_SelectedOption_NotProvided()
    {
        // Arrange
        var model = new AddressForNoticesViewModel();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SelectedAddressOptions);
    }
}

