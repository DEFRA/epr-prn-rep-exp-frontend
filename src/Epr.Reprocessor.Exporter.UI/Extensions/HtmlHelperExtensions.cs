using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epr.Reprocessor.Exporter.UI.Extensions
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Returns the application title based on the current controller name. Used to correctly populate 
        /// the text in the site header and browser title, e.g. "Registration", "Accreditation", etc.
        /// Other logic can be added here if necessary. The returned value is not localised - i.e. pass to 
        /// a localiser in the view.
        /// </summary>
        /// <param name="htmlHelper">The htmlHelper.</param>
        /// <returns>The non-localised application title.</returns>
        public static string ApplicationTitle(this IHtmlHelper htmlHelper)
        {
            string controllerName = htmlHelper.ViewContext.RouteData.Values["controller"].ToString();

            // Add any required logic.
            return controllerName switch
            {
                "Home" => string.Empty,
                _ => controllerName
            };
        }
    }
}
