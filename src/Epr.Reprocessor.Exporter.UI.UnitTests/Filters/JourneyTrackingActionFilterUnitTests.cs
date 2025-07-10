using Epr.Reprocessor.Exporter.UI.Filters;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Filters;

public class StubController
{
    [JourneyTrackingOptions(ExcludeFromTracking = true, PagePath = "/custom/path")]
    public void StubAction() {}
}

[TestClass]
public class JourneyTrackerActionFilterUnitTests
{
    public class FakeSession : ISessionData
    {
        public List<string> Journey { get; set; } = new();
    }

    private Mock<ISessionManager<FakeSession>> _sessionManager = null!;
    private JourneyTrackerActionFilter<FakeSession> _filter = null!;
    private FakeSession _session = null!;

    [TestInitialize]
    public void Setup()
    {
        _sessionManager = new Mock<ISessionManager<FakeSession>>();
        _session = new FakeSession();
        _filter = new JourneyTrackerActionFilter<FakeSession>(_sessionManager.Object);
    }

    [TestMethod]
    public async Task CreatesNewSession_WhenNoneExists()
    {
        // Arrange
        var context = TestContext("/material/review", typeof(StubController));

        // Expectations
        _sessionManager.Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync((FakeSession)null!);

        _sessionManager.Setup(m => m.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<FakeSession>()))
            .Verifiable();

        // Act
        await _filter.OnActionExecutionAsync(context, () => Task.FromResult(new ActionExecutedContext(context, [], new object())));

        // Assert
        _sessionManager.Verify();
    }

    [TestMethod]
    public async Task SkipsTracking_IfAttributeExcluded()
    {
        // Arrange
        var context = TestContext("/material/healthcheck", typeof(StubController));

        // Expectations
        _sessionManager.Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(_session);

        // Act
        await _filter.OnActionExecutionAsync(context, () => Task.FromResult(new ActionExecutedContext(context, [], new object())));

        // Assert
        Assert.IsEmpty(_session.Journey);
    }

    private static ActionExecutingContext TestContext(string path, Type controller)
    {
        var httpContext = new DefaultHttpContext
        {
            Request = { Path = path },
            Session = new Mock<ISession>().Object
        };

        var descriptor = CreateControllerDescriptor(controller);

        return new ActionExecutingContext(
            new ActionContext(httpContext, new(), descriptor),
            [],
            new Dictionary<string, object?>(),
            new object());
    }

    private static ControllerActionDescriptor CreateControllerDescriptor(Type controller)
    {
        var methodInfo = controller.GetMethod("StubAction")!;
        var descriptor = new ControllerActionDescriptor
        {
            MethodInfo = methodInfo
        };
        return descriptor;
    }
}