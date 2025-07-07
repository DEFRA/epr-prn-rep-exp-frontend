using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Epr.Reprocessor.Exporter.UI.ViewComponents;

/// <summary>
/// Defines a view component that renders a GDS compliant summary list component.
/// </summary>
public class SummaryListViewComponent : ViewComponent
{
    /// <summary>
    /// Invokes the view component and renders the resulting HTML.
    /// </summary>
    /// <param name="model">The model associated with this component, used to power the rendering of the view.</param>
    /// <returns>The view component rendered result.</returns>
    public ViewViewComponentResult Invoke(SummaryListModel model) => View(model);
}