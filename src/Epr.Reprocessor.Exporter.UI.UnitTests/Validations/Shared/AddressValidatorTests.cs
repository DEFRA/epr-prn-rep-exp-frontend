using AutoFixture;
using Epr.Reprocessor.Exporter.UI.Validations.Shared;
using FluentValidation.TestHelper;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Validations.Shared;

[TestClass]
public class AddressValidatorTests
{
    private AddressValidator _validator;
    private Fixture _fixture;

    [TestInitialize]
    public void Setup()
    {
        _validator = new AddressValidator();

        _fixture = new Fixture();
    }

    [TestMethod]
    public void ShouldNotHaveError_When_ValidDataProvided()
    {
        // Arrange
        var model = _fixture.Build<AddressViewModel>()
                .With(x => x.AddressLine1, "Some street address")
                .With(x => x.AddressLine2, string.Empty)
                .With(x => x.TownOrCity, "Some town or city")
                .With(x => x.County, string.Empty)
                .With(x => x.Postcode, "G12 3GX")
                .Create();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestMethod]
    public void ShouldHaveError_When_AddressLine1_IsEmpty()
    {
        // Arrange
        var model = _fixture.Build<AddressViewModel>()
            .With(x => x.AddressLine1, string.Empty)
            .Create();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AddressLine1);
    }

    [TestMethod]
    public void ShouldHaveError_When_AddressLine1_ExceedsMaxLength()
    {
        // Arrange
        var model = _fixture.Build<AddressViewModel>()
            .With(x => x.AddressLine1, new string('x', MaxLengths.AddressLine1 + 1))
            .Create();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AddressLine1);
    }

    [TestMethod]
    public void ShouldHaveError_When_AddressLine2_ExceedsMaxLength()
    {
        // Arrange
        var model = _fixture.Build<AddressViewModel>()
            .With(x => x.AddressLine2, new string('x', MaxLengths.AddressLine2 + 1))
            .Create();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AddressLine2);
    }

    [TestMethod]
    public void ShouldHaveError_When_TownOrCity_IsEmpty()
    {
        // Arrange
        var model = _fixture.Build<AddressViewModel>()
            .With(x => x.TownOrCity, string.Empty)
            .Create();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TownOrCity);
    }

    [TestMethod]
    public void ShouldHaveError_When_TownOrCity_ExceedsMaxLength()
    {
        // Arrange
        var model = _fixture.Build<AddressViewModel>()
            .With(x => x.TownOrCity, new string('x', MaxLengths.TownOrCity + 1))
            .Create();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TownOrCity);
    }

    [TestMethod]
    public void ShouldHaveError_When_County_ExceedsMaxLength()
    {
        // Arrange
        var model = _fixture.Build<AddressViewModel>()
            .With(x => x.County, new string('x', MaxLengths.County + 1))
            .Create();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.County);
    }

    [TestMethod]
    public void ShouldHaveError_When_Postcode_IsEmpty()
    {
        // Arrange
        var model = _fixture.Build<AddressViewModel>()
            .With(x => x.Postcode, string.Empty)
            .Create();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Postcode);
    }


    [TestMethod]
    public void ShouldHaveError_When_Postcode_IsInvalid()
    {
        // Arrange
        var model = _fixture.Build<AddressViewModel>()
            .With(x => x.Postcode, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
            .Create();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Postcode);
    }
}