using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class TypeOfSuppliersViewModel
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "List the type of suppliers you get this packaging waste from")]
    [MaxLength(500, ErrorMessage = "The list must be 500 characters or less")]
    public string? TypeOfSuppliers { get; set; }

    public string? MaterialName { get; set; }

    public void MapForView(string typeOfSuppliers, string materialName)
    {
        this.TypeOfSuppliers = typeOfSuppliers;
        this.MaterialName = materialName;
    }
}