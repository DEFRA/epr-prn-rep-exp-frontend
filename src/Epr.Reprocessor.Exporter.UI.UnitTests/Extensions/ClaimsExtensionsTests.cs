using AutoFixture;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Extensions;

[TestClass]
public class ClaimsExtensionsTests
{
    private IFixture _fixture;

    [TestInitialize]
    public void SetUp()
    {
        _fixture = new Fixture();
    }

    [TestMethod]
    public void GetUserData_ShouldThrow_WhenNoUserDataClaimPresent()
    {
        // Arrange
        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(x => x.Claims).Returns(Enumerable.Empty<Claim>());

        // Act
        Action act = () => claimsPrincipalMock.Object.GetUserData();

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void GetUserData_ShouldReturnUserData_WhenUserDataClaimPresent()
    {
        // Arrange
        var userData = _fixture.Create<UserData>();
        string serializedUserData = JsonSerializer.Serialize(userData);

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(x => x.Claims).Returns(new[] { new Claim(ClaimTypes.UserData, serializedUserData) });

        // Act
        var result = claimsPrincipalMock.Object.GetUserData();

        // Assert
        result.Should().BeEquivalentTo(userData);
    }

    [TestMethod]
    public void GetUserData_ShouldThrow_WhenUserDataClaimIsMalformed()
    {
        // Arrange
        string malformedUserData = "{this is not valid JSON}";

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(x => x.Claims).Returns(new[] { new Claim(ClaimTypes.UserData, malformedUserData) });

        // Act
        Action act = () => claimsPrincipalMock.Object.GetUserData();

        // Assert
        act.Should().Throw<JsonException>();
    }
}