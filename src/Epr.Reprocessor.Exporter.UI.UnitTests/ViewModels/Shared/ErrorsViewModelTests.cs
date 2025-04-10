using Epr.Reprocessor.Exporter.UI.ViewModels.Shared.GovUk;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Moq;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels.Shared.GovUk.Tests;

[TestClass]
public class ErrorsViewModelTests
{
    private Mock<IStringLocalizer<SharedResources>> _stringLocalizerMock;
    private Mock<IViewLocalizer> _viewLocalizerMock;

    [TestInitialize]
    public void Setup()
    {
        _stringLocalizerMock = new Mock<IStringLocalizer<SharedResources>>();
        _viewLocalizerMock = new Mock<IViewLocalizer>();
    }

    [TestMethod]
    public void Constructor_ShouldInitializeErrors()
    {
        // Arrange
        var errors = new Dictionary<string, List<ErrorViewModel>>
        {
            { "Field1", new List<ErrorViewModel> { new ErrorViewModel { Key = "Field1", Message = "Error1" } } }
        };
        _stringLocalizerMock.Setup(l => l[It.IsAny<string>()]).Returns(new LocalizedString("Error1", "LocalizedError1"));

        // Act
        var viewModel = new ErrorsViewModel(errors, _stringLocalizerMock.Object);

        // Assert
        Assert.IsNotNull(viewModel.Errors);
        Assert.AreEqual("LocalizedError1", viewModel.Errors["Field1"].First().Message);
    }

    [TestMethod]
    public void Indexer_ShouldReturnErrorsForKey()
    {
        // Arrange
        var errors = new Dictionary<string, List<ErrorViewModel>>
        {
            { "Field1", new List<ErrorViewModel> { new ErrorViewModel { Key = "Field1", Message = "Error1" } } }
        };
        _stringLocalizerMock.Setup(l => l[It.IsAny<string>()]).Returns(new LocalizedString("Error1", "LocalizedError1"));
        var viewModel = new ErrorsViewModel(errors, _stringLocalizerMock.Object);

        // Act
        var result = viewModel["Field1"];

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("LocalizedError1", result.First().Message);
    }

    [TestMethod]
    public void HasErrorKey_ShouldReturnTrueIfKeyExists()
    {
        // Arrange
        var errors = new Dictionary<string, List<ErrorViewModel>>
        {
            { "Field1", new List<ErrorViewModel> { new ErrorViewModel { Key = "Field1", Message = "Error1" } } }
        };
        _stringLocalizerMock.Setup(l => l[It.IsAny<string>()]).Returns(new LocalizedString("Error1", "LocalizedError1"));
        var viewModel = new ErrorsViewModel(errors, _stringLocalizerMock.Object);

        // Act
        var result = viewModel.HasErrorKey("Field1");

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void HasErrorKey_ShouldReturnFalseIfKeyDoesNotExist()
    {
        // Arrange
        var errors = new Dictionary<string, List<ErrorViewModel>>
        {
            { "Field1", new List<ErrorViewModel> { new ErrorViewModel { Key = "Field1", Message = "Error1" } } }
        };
        _stringLocalizerMock.Setup(l => l[It.IsAny<string>()]).Returns(new LocalizedString("Error1", "LocalizedError1"));
        var viewModel = new ErrorsViewModel(errors, _stringLocalizerMock.Object);

        // Act
        var result = viewModel.HasErrorKey("Field2");

        // Assert
        Assert.IsFalse(result);
    }
}
