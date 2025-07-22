namespace Epr.Reprocessor.Exporter.UI.UnitTests.Services;

[TestClass]
public class NationAccessorUnitTests : HttpContextAwareUnitTestBase
{
    [TestMethod]
    public void GetNation_NullConstructorSessionParameter_ThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            _ = new NationAccessor(null!, MockHttpContextAccessor.Object);
        });
    }

    [TestMethod]
    public void GetNation_NullConstructorHttpContextAccessorParameter_ThrowException()
    {
        // Arrange
        var mockSessionManager = new Mock<ISessionManager<ReprocessorRegistrationSession>>();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            _ = new NationAccessor(mockSessionManager.Object, null!);
        });
    }

    [TestMethod]
    public async Task GetNation_NullSession_ReturnNullNation()
    {
        // Arrange
        var mockSessionManager = new Mock<ISessionManager<ReprocessorRegistrationSession>>();
        var sut = new NationAccessor(mockSessionManager.Object, MockHttpContextAccessor.Object);

        // Expectations
        mockSessionManager.Setup(o => o.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync((ReprocessorRegistrationSession?)null);

        // Act
        var result = await sut.GetNation();

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task GetNation_SessionExists_NullReprocessingSite_ReturnNullNation()
    {
        // Arrange
        var mockSessionManager = new Mock<ISessionManager<ReprocessorRegistrationSession>>();
        var sut = new NationAccessor(mockSessionManager.Object, MockHttpContextAccessor.Object);

        // Expectations
        mockSessionManager.Setup(o => o.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorRegistrationSession
            {
                RegistrationApplicationSession = new RegistrationApplicationSession
                {
                    ReprocessingSite = null
                }
            });

        // Act
        var result = await sut.GetNation();

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task GetNation_SessionExists_ReprocessingSiteExists_DifferentAddress_ReturnNationFromSessionAsProvidedByUser()
    {
        // Arrange
        var mockSessionManager = new Mock<ISessionManager<ReprocessorRegistrationSession>>();
        var sut = new NationAccessor(mockSessionManager.Object, MockHttpContextAccessor.Object);

        // Expectations
        mockSessionManager.Setup(o => o.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(new ReprocessorRegistrationSession
            {
                RegistrationApplicationSession = new RegistrationApplicationSession
                {
                    ReprocessingSite = new ReprocessingSite
                    {
                        TypeOfAddress = AddressOptions.DifferentAddress,
                        Nation = UkNation.NorthernIreland
                    }
                }
            });

        // Act
        var result = await sut.GetNation();

        // Assert
        result.Should().Be(UkNation.NorthernIreland);
    }

    [TestMethod]
    [DataRow(AddressOptions.BusinessAddress)]
    [DataRow(AddressOptions.RegisteredAddress)]
    public async Task GetNation_SessionExists_ReprocessingSiteExists_BusinessOrRegisteredAddress_ReturnNationFromClaims(AddressOptions typeOfAddress)
    {
        // Arrange
        var mockSessionManager = new Mock<ISessionManager<ReprocessorRegistrationSession>>();
        var sut = new NationAccessor(mockSessionManager.Object, MockHttpContextAccessor.Object);

        // Expectations
        mockSessionManager.Setup(o => o.GetSessionAsync(MockSession.Object))
            .ReturnsAsync(new ReprocessorRegistrationSession
            {
                RegistrationApplicationSession = new RegistrationApplicationSession
                {
                    ReprocessingSite = new ReprocessingSite
                    {
                        TypeOfAddress = typeOfAddress
                    }
                }
            });

        // Act
        var result = await sut.GetNation();

        // Assert
        result.Should().Be(UkNation.England);
    }

    [TestMethod]
    public async Task GetNation_SessionExists_ReprocessingSiteExists_NationNull_RetunrFromClaims()
    {
        // Arrange
        var mockSessionManager = new Mock<ISessionManager<ReprocessorRegistrationSession>>();
        var sut = new NationAccessor(mockSessionManager.Object, MockHttpContextAccessor.Object);

        // Expectations
        mockSessionManager.Setup(o => o.GetSessionAsync(MockSession.Object))
            .ReturnsAsync(new ReprocessorRegistrationSession
            {
                RegistrationApplicationSession = new RegistrationApplicationSession
                {
                    ReprocessingSite = new ReprocessingSite()
                }
            });

        // Act
        var result = await sut.GetNation();

        // Assert
        result.Should().Be(UkNation.England);
    }

    [TestMethod]
    public async Task GetNation_SessionExists_ReprocessingSiteExists_NationNotRegistreredOrBusiness_ReturnFromSession()
    {
        // Arrange
        var mockSessionManager = new Mock<ISessionManager<ReprocessorRegistrationSession>>();
        var sut = new NationAccessor(mockSessionManager.Object, MockHttpContextAccessor.Object);

        // Expectations
        mockSessionManager.Setup(o => o.GetSessionAsync(MockSession.Object))
            .ReturnsAsync(new ReprocessorRegistrationSession
            {
                RegistrationApplicationSession = new RegistrationApplicationSession
                {
                    ReprocessingSite = new ReprocessingSite
                    {
                        Nation = UkNation.Scotland,
                        TypeOfAddress = AddressOptions.DifferentAddress
                    }
                }
            });

        // Act
        var result = await sut.GetNation();

        // Assert
        result.Should().Be(UkNation.Scotland);
    }
}