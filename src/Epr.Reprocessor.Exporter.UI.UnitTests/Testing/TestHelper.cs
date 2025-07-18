using Microsoft.AspNetCore.Mvc.Filters;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Testing;

public static class TestHelper
{
    /// <summary>
    /// Creates a minimal ActionExecutingContext with the given controller instance.
    /// Useful for testing filters, resolvers, and providers.
    /// </summary>
    /// <param name="controller">The controller object (real or mocked).</param>
    public static ActionExecutingContext CreateContext(object controller)
    {
        var httpContext = new DefaultHttpContext();

        var routeData = new RouteData
        {
            Values =
            {
                ["controller"] = controller.GetType().Name.Replace("Controller", ""),
                ["action"] = "Test"
            }
        };

        var actionDescriptor = new ControllerActionDescriptor
        {
            ControllerName = routeData.Values["controller"]!.ToString()!,
            ActionName = routeData.Values["action"]!.ToString()!
        };

        var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);

        return new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object>()!,
            controller
        );
    }
}