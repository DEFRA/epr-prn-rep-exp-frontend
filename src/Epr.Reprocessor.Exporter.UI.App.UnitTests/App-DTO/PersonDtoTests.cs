using Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.App_DTO
{
    [TestClass]
    public class PersonDtoTests
    {
        [TestMethod]
        public void TestFirstNameProperty()
        {
            // Arrange
            var person = new PersonDto();
            var expectedFirstName = "John";

            // Act
            person.FirstName = expectedFirstName;
            var actualFirstName = person.FirstName;

            // Assert
            Assert.AreEqual(expectedFirstName, actualFirstName);
        }

        [TestMethod]
        public void TestLastNameProperty()
        {
            // Arrange
            var person = new PersonDto();
            var expectedLastName = "Doe";

            // Act
            person.LastName = expectedLastName;
            var actualLastName = person.LastName;

            // Assert
            Assert.AreEqual(expectedLastName, actualLastName);
        }

        [TestMethod]
        public void TestContactEmailProperty()
        {
            // Arrange
            var person = new PersonDto();
            var expectedContactEmail = "john.doe@example.com";

            // Act
            person.ContactEmail = expectedContactEmail;
            var actualContactEmail = person.ContactEmail;

            // Assert
            Assert.AreEqual(expectedContactEmail, actualContactEmail);
        }
    }
}
