using Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.App_DTO;

[TestClass]
public class UserAccountDtoTests
{
    [TestMethod]
    public void UserAccountDto_UserProperty_GetSet()
    {
        // Arrange
        var userAccountDto = new UserAccountDto();
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            RoleInOrganisation = "Admin",
            EnrolmentStatus = "Active",
            ServiceRole = "Manager",
            ServiceRoleId = 1,
            Service = "IT",
            Organisations = new List<Organisation>
            {
                new Organisation
                {
                    Id = Guid.NewGuid(),
                    OrganisationName = "Org1",
                    OrganisationRole = "Role1",
                    OrganisationType = "Type1"
                }
            }
        };

        // Act
        userAccountDto.User = user;

        // Assert
        Assert.AreEqual(user, userAccountDto.User);
    }
}
