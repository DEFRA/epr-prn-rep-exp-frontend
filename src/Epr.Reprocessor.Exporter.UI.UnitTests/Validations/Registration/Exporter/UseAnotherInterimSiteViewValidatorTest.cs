using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using Epr.Reprocessor.Exporter.UI.App.Domain;
using Epr.Reprocessor.Exporter.UI.Validations.Registration;
using Epr.Reprocessor.Exporter.UI.Validations.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

using FluentValidation.TestHelper;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Validations.Registration.Exporter;

[TestClass]
public  class UseAnotherInterimSiteViewValidatorTest
{

    private UseAnotherInterimSiteViewValidator _validator;
    private Fixture _fixture;

    [TestInitialize]
    public void Setup()
    {
        _validator = new UseAnotherInterimSiteViewValidator();
        _fixture = new Fixture();
    }

    [TestMethod]
    public void ShouldNotHaveAnyErrors_After_Validation()
    {
        // Arrange
        var model = _fixture.Build<UseAnotherInterimSiteViewModel>()
                    .With(x => x.AddInterimSiteAccepted).Create();

        // Act
        var result = _validator.TestValidate(model);


        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestMethod]
    public void ShouldHaveError_When_Option_is_NotSelected()
    {
        // Arrange
        var model = new UseAnotherInterimSiteViewModel { AddInterimSiteAccepted = null };


        // Act
        var result = _validator.TestValidate(model);


        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AddInterimSiteAccepted);
    }
}

