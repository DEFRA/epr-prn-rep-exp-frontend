using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs;

[ExcludeFromCodeCoverage]
public class AddressDto
{
    public int? Id { get; set; }

    public string AddressLine1 { get; set; }

    public string AddressLine2 { get; set; }

    public string TownCity { get; set; }

    public string County { get; set; }

    public string Country { get; set; }

    public string PostCode { get; set; }

    public int NationId { get; set; }

    public string GridReference { get; set; }
}