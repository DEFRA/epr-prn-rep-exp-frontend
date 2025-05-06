using AutoFixture;
using Epr.Reprocessor.Exporter.UI.Validations.Shared;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;
using FluentValidation.TestHelper;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Validations.Shared;

[TestClass]
public class SelectedAddressValidatorTests
{
    private SelectedAddressValidator _validator;
    private Fixture _fixture;

    [TestInitialize]
    public void Setup()
    {
        _validator = new SelectedAddressValidator();

        _fixture = new Fixture();
    }

    [TestMethod]
    public void ShouldNotHaveError_When_ValidDataProvided()
    {
        // Arrange
        var model = _fixture.Build<SelectedAddressViewModel>()
                .With(x => x.SelectedIndex, 1)
                .With(x => x.Postcode, "G12 3GX")
                .Create();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestMethod]
    public void ShouldHaveError_When_SelectedIndex_IsNull()
    {
        // Arrange
        var model = new SelectedAddressViewModel
        {
            Postcode = "G12 3GX",
            SelectedIndex = null
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SelectedIndex);
    }
}