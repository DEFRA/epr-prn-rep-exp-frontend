using AutoFixture;
using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.Validations.Registration;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration;
using FluentValidation.TestHelper;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Validations.Registration;

[TestClass]
public class ManualAddressForReprocessingSiteValidatorTests
{
    private ManualAddressForReprocessingSiteValidator _validator;
    private Fixture _fixture;

    [TestInitialize]
    public void Setup()
    {
        _validator = new ManualAddressForReprocessingSiteValidator();

        _fixture = new Fixture();
    }

    [TestMethod]
    public void ShouldNotHaveError_When_ValidDataProvided()
    {
        // Arrange
        var model = _fixture.Build<ManualAddressForReprocessingSiteViewModel>()
                .With(x => x.AddressLine1, "Some street address")
                .With(x => x.AddressLine2, string.Empty)
                .With(x => x.TownOrCity, "Some town or city")
                .With(x => x.County, string.Empty)
                .With(x => x.Postcode, "G12 3GX")
                .With(x => x.SiteGridReference, "AB12345678")
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
        var model = _fixture.Build<ManualAddressForReprocessingSiteViewModel>()
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
        var model = _fixture.Build<ManualAddressForReprocessingSiteViewModel>()
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
        var model = _fixture.Build<ManualAddressForReprocessingSiteViewModel>()
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
        var model = _fixture.Build<ManualAddressForReprocessingSiteViewModel>()
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
        var model = _fixture.Build<ManualAddressForReprocessingSiteViewModel>()
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
        var model = _fixture.Build<ManualAddressForReprocessingSiteViewModel>()
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
        var model = _fixture.Build<ManualAddressForReprocessingSiteViewModel>()
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
        var model = _fixture.Build<ManualAddressForReprocessingSiteViewModel>()
            .With(x => x.Postcode, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
            .Create();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Postcode);
    }

    [TestMethod]
    public void ShouldHaveError_When_SiteGridReference_IsEmpty()
    {
        // Arrange
        var model = _fixture.Build<ManualAddressForReprocessingSiteViewModel>()
            .With(x => x.SiteGridReference, string.Empty)
            .Create();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SiteGridReference);
    }

    [TestMethod]
    public void ShouldHaveError_When_SiteGridReference_ExceedsMaxLength()
    {
        // Arrange
        var model = _fixture.Build<ManualAddressForReprocessingSiteViewModel>()
            .With(x => x.SiteGridReference, new string('x', 11))
            .Create();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SiteGridReference);
    }

    [TestMethod]
    public void ShouldHaveError_When_SiteGridReference_MinLength()
    {
        // Arrange
        var model = _fixture.Build<ManualAddressForReprocessingSiteViewModel>()
            .With(x => x.SiteGridReference, "123")
            .Create();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SiteGridReference);
    }

    [TestMethod]
    public void ShouldHaveError_When_SiteGridReference_Invalid()
    {
        // Arrange
        var model = _fixture.Build<ManualAddressForReprocessingSiteViewModel>()
            .With(x => x.SiteGridReference, "@123X£$@@@")
            .Create();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SiteGridReference);
    }
}