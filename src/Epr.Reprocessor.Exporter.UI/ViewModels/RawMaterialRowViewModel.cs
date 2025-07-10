using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class RawMaterialRowViewModel
{
    public string RawMaterialName { get; set; }
    public string? Tonnes { get; set; }
}