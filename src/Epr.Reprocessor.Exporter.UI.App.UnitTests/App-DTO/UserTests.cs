using Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.App_DTO;

[TestClass]
public class UserTests
{
    [TestMethod]
    public void TestUserProperties()
    {
        // Arrange
        var user = new User();
        var id = Guid.NewGuid();
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var roleInOrganisation = "Manager";
        var enrolmentStatus = "Active";
        var serviceRole = "Admin";
        var serviceRoleId = 1;
        var service = "IT";
        var organisations = new List<Organisation>
        {
            new Organisation
            {
                Id = Guid.NewGuid(),
                OrganisationName = "Org1",
                OrganisationRole = "Role1",
                OrganisationType = "Type1"
            }
        };

        // Act
        user.Id = id;
        user.FirstName = firstName;
        user.LastName = lastName;
        user.Email = email;
        user.RoleInOrganisation = roleInOrganisation;
        user.EnrolmentStatus = enrolmentStatus;
        user.ServiceRole = serviceRole;
        user.ServiceRoleId = serviceRoleId;
        user.Service = service;
        user.Organisations = organisations;

        // Assert
        Assert.AreEqual(id, user.Id);
        Assert.AreEqual(firstName, user.FirstName);
        Assert.AreEqual(lastName, user.LastName);
        Assert.AreEqual(email, user.Email);
        Assert.AreEqual(roleInOrganisation, user.RoleInOrganisation);
        Assert.AreEqual(enrolmentStatus, user.EnrolmentStatus);
        Assert.AreEqual(serviceRole, user.ServiceRole);
        Assert.AreEqual(serviceRoleId, user.ServiceRoleId);
        Assert.AreEqual(service, user.Service);
        Assert.AreEqual(organisations, user.Organisations);
    }
}
