using Epr.Reprocessor.Exporter.UI.Validations.Accreditation;
using Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;
using FluentValidation.TestHelper;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Validations.Accreditation
{
    [TestClass]
    public class SelectAuthorityValidatorTests
    {
        private SelectAuthorityValidator _validator;

        [TestInitialize]
        public void Setup()
        {
            _validator = new SelectAuthorityValidator();
        }

        [TestMethod]
        public void Should_Have_Error_When_SelectedAuthorities_Is_Empty()
        {
            // Arrange
            var model = new SelectAuthorityViewModel
            {
                SelectedAuthorities = new List<string>()
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SelectedAuthoritiesCount);
        }

        [TestMethod]
        public void Should_Not_Have_Error_When_SelectedAuthorities_Has_One_Item()
        {
            // Arrange
            var model = new SelectAuthorityViewModel
            {
                SelectedAuthorities = new List<string> { "Authority1" }
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.SelectedAuthorities);
        }

        [TestMethod]
        public void Should_Not_Have_Error_When_SelectedAuthorities_Has_Multiple_Items()
        {
            // Arrange
            var model = new SelectAuthorityViewModel
            {
                SelectedAuthorities = new List<string> { "Authority1", "Authority2" }
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.SelectedAuthorities);
        }
    }
}
