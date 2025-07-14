namespace Epr.Reprocessor.Exporter.UI.UnitTests.Domain;

[TestClass]
public class ReprocessorRegistrationSessionUnitTests
{
    [TestMethod]
    public void SetFromExisting_NoReprocessingSiteAddress_ThrowException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var sut = new ReprocessorRegistrationSession
        {
            RegistrationId = id,
            RegistrationApplicationSession = new()
            {
                ReprocessingSite = null
            }
        };
       
        var registrationDto = new RegistrationDto
        {
            Id = id,
            ReprocessingSiteAddress = null,
            LegalDocumentAddress = new AddressDto
            {
                AddressLine1 = "Another Address line 1",
                AddressLine2 = "Another Address line 2",
                Country = "Another country",
                County = "Another county",
                TownCity = "Another town",
                PostCode = "Another postcode"
            }
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => sut.SetFromExisting(registrationDto, false));
    }

    [TestMethod]
    public void SetFromExisting_EnsureCorrectValuesSet()
    {
        // Arrange
        var id = Guid.NewGuid();
        var sut = new ReprocessorRegistrationSession
        {
            RegistrationId = id
        };
        var expected = new ReprocessorRegistrationSession
        {
            RegistrationId = id,
            RegistrationApplicationSession = new()
            {
                ReprocessingSite = new ReprocessingSite
                {
                    Address = new Address("Address line 1", "Address line 2", null, "town", "county", "country", "postcode"),
                    ServiceOfNotice = new ServiceOfNotice
                    {
                        Address = new Address("Another Address line 1", "Another Address line 2", null, "Another town", "Another county", "Another country", "Another postcode"),
                        TypeOfAddress = AddressOptions.DifferentAddress
                    },
                    SiteGridReference = "123456",
                    TypeOfAddress = AddressOptions.DifferentAddress
                }
            }
        };

        var registrationDto = new RegistrationDto
        {
            Id = id,
            ReprocessingSiteAddress = new AddressDto
            {
                AddressLine1 = "Address line 1",
                AddressLine2 = "Address line 2",
                Country = "country",
                County = "county",
                TownCity = "town",
                PostCode = "postcode",
                GridReference = "123456"
            },
            LegalDocumentAddress = new AddressDto
            {
                AddressLine1 = "Another Address line 1",
                AddressLine2 = "Another Address line 2",
                Country = "Another country",
                County = "Another county",
                TownCity = "Another town",
                PostCode = "Another postcode"
            }
        };

        // Act
        sut.SetFromExisting(registrationDto, false);

        // Assert
        sut.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void SetFromExisting_NoLegalAddress_EnsureCorrectValuesSet()
    {
        // Arrange
        var id = Guid.NewGuid();
        var sut = new ReprocessorRegistrationSession
        {
            RegistrationId = id
        };
        var expected = new ReprocessorRegistrationSession
        {
            RegistrationId = id,
            RegistrationApplicationSession = new()
            {
                ReprocessingSite = new ReprocessingSite
                {
                    Address = new Address("Address line 1", "Address line 2", null, "town", "county", "country", "postcode"),
                    SiteGridReference = "123456",
                    TypeOfAddress = AddressOptions.BusinessAddress
                }
            }
        };

        var registrationDto = new RegistrationDto
        {
            Id = id,
            ReprocessingSiteAddress = new AddressDto
            {
                AddressLine1 = "Address line 1",
                AddressLine2 = "Address line 2",
                Country = "country",
                County = "county",
                TownCity = "town",
                PostCode = "postcode",
                GridReference = "123456"
            }
        };

        // Act
        sut.SetFromExisting(registrationDto, false);

        // Assert
        sut.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void CreateRegistration_EnsureRegistrationIdSet()
    {
        // Arrange
        var registrationId = Guid.NewGuid();
        var sut = new ReprocessorRegistrationSession();

        // Act
        sut.CreateRegistration(registrationId);
        
        // Assert
        sut.RegistrationId.Should().Be(registrationId);
    }
}