using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Testing;

public class HttpContextAwareUnitTestBase
{
    public Mock<HttpContext> MockHttpContext { get; set; } = null!;

    public Mock<IHttpContextAccessor> MockHttpContextAccessor { get; set; } = null!;

    public Mock<ISession> MockSession { get; set; } = null!;

    [TestInitialize]
    public void Setup()
    {
        ConfigureMocks();
    }

    protected virtual void ConfigureMocks()
    {
        MockHttpContext = new Mock<HttpContext>();
        MockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        MockSession = new Mock<ISession>();

        ConfigureMockExpectations();
        ConfigureUser();
    }

    protected virtual void ConfigureMockExpectations()
    {
        ConfigureUser();
        ConfigureHttpContextAccessor();
    }

    protected virtual void ConfigureHttpContextAccessor()
    {
        MockHttpContextAccessor.Setup(o => o.HttpContext).Returns(MockHttpContext.Object);
        MockHttpContext.Setup(o => o.Session).Returns(MockSession.Object);
    }
    
    protected virtual void ConfigureUser()
    {
        var userData = NewUserData().Build();
        MockHttpContext.Setup(o => o.User).Returns(new ClaimsPrincipal(new List<ClaimsIdentity>
            { new([new Claim(ClaimTypes.UserData, JsonSerializer.Serialize(userData))]) }));
    }
}