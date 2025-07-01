using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

[ExcludeFromCodeCoverage]
public class OverseasAddressWasteCodesViewModel
{
    public Guid? Id { get; set; }
    [MaxLength(10)]
    public string? CodeName { get; set; }
}