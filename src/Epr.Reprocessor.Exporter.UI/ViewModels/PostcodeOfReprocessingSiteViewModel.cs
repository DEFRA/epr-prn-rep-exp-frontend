using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class PostcodeOfReprocessingSiteViewModel
{
    public PostcodeOfReprocessingSiteViewModel()
    {

    }

    public PostcodeOfReprocessingSiteViewModel(string postcode)
    {
        Postcode = postcode;
    }

    public string Postcode { get; set; }
}
