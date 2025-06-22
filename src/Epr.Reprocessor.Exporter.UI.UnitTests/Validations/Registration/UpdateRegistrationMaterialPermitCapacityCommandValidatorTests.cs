using Epr.Reprocessor.Exporter.UI.Validations.Registration;
using FluentValidation.TestHelper;

namespace Epr.Reprocessor.Exporter.UnitTests.Validations.Registration;

[TestClass]
public class UpdateRegistrationMaterialPermitCapacityCommandValidatorTests
{
    private UpdateRegistrationMaterialPermitCapacityCommandValidator _validator;

    [TestInitialize]
    public void Setup()
    {
        _validator = new UpdateRegistrationMaterialPermitCapacityCommandValidator();
    }

    [TestMethod]
    public void Should_Pass_When_All_Fields_Are_Valid()
    {
        var model = new UpdateRegistrationMaterialPermitCapacityDto
        {
            PermitTypeId = 2,
            CapacityInTonnes = 5000,
            PeriodId =2
        };

        var result = _validator.TestValidate(model);

        result.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public void Should_Have_Error_When_PermitTypeId_Is_Empty()
    {
        var model = new UpdateRegistrationMaterialPermitCapacityDto
        {
            PermitTypeId = 0,
            CapacityInTonnes = 5000,
            PeriodId = 2
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.PermitTypeId)
            .WithErrorMessage("PermitTypeId is required");
    }

    [TestMethod]
    public void Should_Have_Error_When_CapacityInTonnes_Is_Null()
    {
        var model = new UpdateRegistrationMaterialPermitCapacityDto
        {
            PermitTypeId = 3,
            CapacityInTonnes = null,
            PeriodId = 3
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.CapacityInTonnes)
            .WithErrorMessage("Weight must be a number greater than 0");
    }

    [TestMethod]
    public void Should_Have_Error_When_CapacityInTonnes_Is_Zero()
    {
        var model = new UpdateRegistrationMaterialPermitCapacityDto
        {
            PermitTypeId = 2,
            CapacityInTonnes = 0,
            PeriodId = 2
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.CapacityInTonnes)
            .WithErrorMessage("Weight must be a number greater than 0");
    }

    [TestMethod]
    public void Should_Have_Error_When_CapacityInTonnes_Exceeds_Maximum()
    {
        var model = new UpdateRegistrationMaterialPermitCapacityDto
        {
            PermitTypeId = 2,
            CapacityInTonnes = 10000000,
            PeriodId = 2
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.CapacityInTonnes)
            .WithErrorMessage("Weight must be a number less than 10,000,000");
    }

    [TestMethod]
    public void Should_Have_Error_When_PeriodId_Is_Empty()
    {
        var model = new UpdateRegistrationMaterialPermitCapacityDto
        {
            PermitTypeId = 2,
            CapacityInTonnes = 100,
            PeriodId = null
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.PeriodId)
            .WithErrorMessage("PeriodId is required");
    }
}
