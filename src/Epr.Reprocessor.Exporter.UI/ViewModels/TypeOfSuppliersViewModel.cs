namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class TypeOfSuppliersViewModel
{
    public string? TypeOfSuppliers { get; set; }

    public void MapForView(string typeOfSuppliers)
    {
        this.TypeOfSuppliers = typeOfSuppliers;
    }
}