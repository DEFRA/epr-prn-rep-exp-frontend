using AutoFixture;
using Epr.Reprocessor.Exporter.UI.Validations.Registration;
using FluentValidation.TestHelper;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Validations.Registration;

[TestClass]
public class PackagingWasteWillReprocessValidatorTests
{
    private PackagingWasteWillReprocessValidator _validator;
    private Fixture _fixture;

    [TestInitialize]
    public void Setup()
    {
        _validator = new PackagingWasteWillReprocessValidator();

        _fixture = new Fixture();
    }

    [TestMethod]
    public void ShouldNotHaveError_When_ValidDataProvided()
    {
        // Arrange
        var model = _fixture.Build<PackagingWasteWillReprocessViewModel>()
                .With(x => x.SelectedRegistrationMaterials, new List<string> { "Steel (R4)", "Wood (R3)" })
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
        var model = new PackagingWasteWillReprocessViewModel();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SelectedRegistrationMaterials);
    }
}
