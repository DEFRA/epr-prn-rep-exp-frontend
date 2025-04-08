using Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epr.Reprocessor.Exporter.UI.Tests.App.Tests.DTO;

[TestClass]
public class OrganisationTests
{
    [TestMethod]
    public void Organisation_IdProperty_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var organisation = new Organisation();
        var expectedId = Guid.NewGuid();

        // Act
        organisation.Id = expectedId;
        var actualId = organisation.Id;

        // Assert
        Assert.AreEqual(expectedId, actualId);
    }

    [TestMethod]
    public void Organisation_OrganisationNameProperty_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var organisation = new Organisation();
        var expectedName = "Test Organisation";

        // Act
        organisation.OrganisationName = expectedName;
        var actualName = organisation.OrganisationName;

        // Assert
        Assert.AreEqual(expectedName, actualName);
    }

    [TestMethod]
    public void Organisation_OrganisationRoleProperty_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var organisation = new Organisation();
        var expectedRole = "Admin";

        // Act
        organisation.OrganisationRole = expectedRole;
        var actualRole = organisation.OrganisationRole;

        // Assert
        Assert.AreEqual(expectedRole, actualRole);
    }

    [TestMethod]
    public void Organisation_OrganisationTypeProperty_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var organisation = new Organisation();
        var expectedType = "Non-Profit";

        // Act
        organisation.OrganisationType = expectedType;
        var actualType = organisation.OrganisationType;

        // Assert
        Assert.AreEqual(expectedType, actualType);
    }
}
