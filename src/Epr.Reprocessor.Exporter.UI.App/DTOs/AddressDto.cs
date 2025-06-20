using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs;

[ExcludeFromCodeCoverage]
public record AddressDto
{
    public int? Id { get; set; } = 0;

    public string AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; } = string.Empty;

    public string TownCity { get; set; }

    public string? County { get; set; } = string.Empty;

    public string? Country { get; set; } = string.Empty;

    public string PostCode { get; set; }

    public int? NationId { get; set; } = 0;

    public string GridReference { get; set; }
}