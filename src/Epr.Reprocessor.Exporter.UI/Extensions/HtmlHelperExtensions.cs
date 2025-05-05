using System.Text.Encodings.Web;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epr.Reprocessor.Exporter.UI.Extensions;

public static class HtmlHelperExtensions
{
    public static IHtmlContent DisplayMultilineAddress(this IHtmlHelper htmlHelper, AddressViewModel address)
    {
        var addressParts = new List<string>();

        if (!string.IsNullOrWhiteSpace(address?.AddressLine1))
        {
            addressParts.Add(HtmlEncoder.Default.Encode(address.AddressLine1));
        }

        if (!string.IsNullOrWhiteSpace(address?.AddressLine2))
        {
            addressParts.Add(HtmlEncoder.Default.Encode(address.AddressLine2));
        }

        if (!string.IsNullOrWhiteSpace(address?.TownOrCity))
        {
            addressParts.Add(HtmlEncoder.Default.Encode(address.TownOrCity));
        }

        if (!string.IsNullOrWhiteSpace(address?.County))
        {
            addressParts.Add(HtmlEncoder.Default.Encode(address.County));
        }

        if (!string.IsNullOrWhiteSpace(address?.Postcode))
        {
            addressParts.Add(HtmlEncoder.Default.Encode(address.Postcode));
        }

        var htmlAddress = string.Join("<br />", addressParts);

        return new HtmlString(htmlAddress);
    }
}
