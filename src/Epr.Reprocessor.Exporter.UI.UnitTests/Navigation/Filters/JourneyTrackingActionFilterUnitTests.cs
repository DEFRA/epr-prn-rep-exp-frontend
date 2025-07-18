using Epr.Reprocessor.Exporter.UI.Navigation.Filters;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Navigation.Filters;

public class StubController
{
    [JourneyTrackingOptions(ExcludeFromTracking = true, PagePath = "/custom/path")]
    public static void StubAction() {}

    [JourneyTrackingOptions(ExcludeFromTracking = false, PagePath = "/custom/path")]
    public static void StubActionTwo() { }

    public static void StubActionThree() { }
}

[TestClass]
public class JourneyTrackerActionFilterUnitTests
{
    public class FakeSession : ISessionData
    {
        public List<string> Journey { get; set; } = new();
    }

    private Mock<ISessionManager<FakeSession>> _sessionManager = null!;
    private JourneyTrackerActionFilterAttribute<FakeSession> _filterAttribute = null!;
    private FakeSession _session = null!;

    [TestInitialize]
    public void Setup()
    {
        _sessionManager = new Mock<ISessionManager<FakeSession>>();
        _session = new FakeSession();
        _filterAttribute = new JourneyTrackerActionFilterAttribute<FakeSession>(_sessionManager.Object);
    }

    [TestMethod]
    public async Task CreatesNewSession_WhenNoneExists()
    {
        // Arrange
        var context = TestContext("/material/review", typeof(StubController), nameof(StubController.StubAction));

        // Expectations
        _sessionManager.Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync((FakeSession)null!);

        _sessionManager.Setup(m => m.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<FakeSession>()))
            .Verifiable();

        // Act
        await _filterAttribute.OnActionExecutionAsync(context, () => Task.FromResult(new ActionExecutedContext(context, [], new object())));

        // Assert
        _sessionManager.Verify();
    }

    [TestMethod]
    public async Task SkipsTracking_IfAttributeExcluded()
    {
        // Arrange
        var context = TestContext("/material/healthcheck", typeof(StubController), nameof(StubController.StubAction));

        // Expectations
        _sessionManager.Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(_session);

        // Act
        await _filterAttribute.OnActionExecutionAsync(context, () => Task.FromResult(new ActionExecutedContext(context, [], new object())));

        // Assert
        _session.Journey.Should().BeEmpty();
    }

    [TestMethod]
    public async Task AttributeExists_DoNotExclude_AddCustomPath()
    {
        // Arrange
        var context = TestContext("/material/healthcheck", typeof(StubController),
            nameof(StubController.StubActionTwo));

        // Expectations
        _sessionManager.Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(_session);

        // Act
        await _filterAttribute.OnActionExecutionAsync(context,
            () => Task.FromResult(new ActionExecutedContext(context, [], new object())));

        // Assert
        _session.Journey.Should().BeEquivalentTo(new List<string>
        {
            "/custom/path"
        });
    }

    [TestMethod]
    public async Task AttributeDoesNotExist_DoNotExclude_AddPath()
    {
        // Arrange
        var context = TestContext("/material/page", typeof(StubController), nameof(StubController.StubActionThree));

        // Expectations
        _sessionManager.Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(_session);

        // Act
        await _filterAttribute.OnActionExecutionAsync(context, () => Task.FromResult(new ActionExecutedContext(context, [], new object())));

        // Assert
        _session.Journey.Should().BeEquivalentTo(new List<string>
        {
            "/material/page"
        });
    }

    [TestMethod]
    public async Task PathAlreadyExistsInSession_DoNotAddAgain()
    {
        // Arrange
        var context = TestContext("/material/page", typeof(StubController), nameof(StubController.StubActionThree));
        _session.Journey = ["/material/page"];

        // Expectations
        _sessionManager.Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(_session);

        // Act
        await _filterAttribute.OnActionExecutionAsync(context, () => Task.FromResult(new ActionExecutedContext(context, [], new object())));

        // Assert
        _session.Journey.Should().BeEquivalentTo(new List<string>
        {
            "/material/page"
        });
    }

    private static ActionExecutingContext TestContext(string path, Type controller, string actionName)
    {
        var httpContext = new DefaultHttpContext
        {
            Request = { Path = path },
            Session = new Mock<ISession>().Object
        };

        var descriptor = CreateControllerDescriptor(controller, actionName);

        return new ActionExecutingContext(
            new ActionContext(httpContext, new(), descriptor),
            [],
            new Dictionary<string, object?>(),
            new object());
    }

    private static ControllerActionDescriptor CreateControllerDescriptor(Type controller, string actionName)
    {
        var methodInfo = controller.GetMethod(actionName)!;
        var descriptor = new ControllerActionDescriptor
        {
            MethodInfo = methodInfo
        };
        return descriptor;
    }
}