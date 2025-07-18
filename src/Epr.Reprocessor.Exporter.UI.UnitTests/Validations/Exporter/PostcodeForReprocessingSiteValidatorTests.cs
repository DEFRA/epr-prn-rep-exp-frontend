using AutoFixture;
using Epr.Reprocessor.Exporter.UI.Validations.ExorterJourney;
using Epr.Reprocessor.Exporter.UI.Validations.Registration;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using FluentValidation.TestHelper;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Validations.Exporter;

[TestClass]
public class PostcodeForReprocessingSiteValidatorTests
{
    private PostcodeForExporterSiteValidator _validator;
    private Fixture _fixture;

    [TestInitialize]
    public void Setup()
    {
        _validator = new PostcodeForExporterSiteValidator();

        _fixture = new Fixture();
    }

    [TestMethod]
    public void ShouldNotHaveError_When_ValidDataProvided()
    {
        // Arrange
        var model = _fixture.Build<AddressSearchViewModel>()
                .With(x => x.Postcode, "G12 3GX")
                .Create();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestMethod]
    public void ShouldHaveError_When_Postcode_IsEmpty()
    {
        // Arrange
        var model = _fixture.Build<AddressSearchViewModel>()
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
        var model = _fixture.Build<AddressSearchViewModel>()
            .With(x => x.Postcode, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
            .Create();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Postcode);
    }
}