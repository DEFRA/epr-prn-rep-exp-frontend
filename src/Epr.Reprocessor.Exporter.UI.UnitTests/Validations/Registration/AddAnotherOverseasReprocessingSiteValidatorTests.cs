using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using Epr.Reprocessor.Exporter.UI.App.Domain;
using Epr.Reprocessor.Exporter.UI.Validations.Registration;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;
using FluentValidation.TestHelper;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Validations.Registration;

[TestClass]
public class AddAnotherOverseasReprocessingSiteValidatorTests
{

    private AddAnotherOverseasReprocessingSiteValidator _validator;
    private Fixture _fixture;

    [TestInitialize]
    public void Setup()
    {
        _validator = new AddAnotherOverseasReprocessingSiteValidator();
        _fixture = new Fixture();
    }


    [TestMethod]
    public void ShouldNotHaveAnyErrors_After_Validation() 
    {
        //Arrange
        var model = _fixture.Build<AddAnotherOverseasReprocessingSiteViewModel>()
                    .With(x => x.AddOverseasSiteAccepted).Create();

        // Act
        var result = _validator.TestValidate(model);


        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }


    [TestMethod]
    public void ShouldHaveError_When_Option_Is_NotSelected()
    {
        // Arrange
        var model = new AddAnotherOverseasReprocessingSiteViewModel { AddOverseasSiteAccepted = null };


        // Act
        var result = _validator.TestValidate(model);


        //Assert
        result.ShouldHaveValidationErrorFor(x => x.AddOverseasSiteAccepted);
    }

}

