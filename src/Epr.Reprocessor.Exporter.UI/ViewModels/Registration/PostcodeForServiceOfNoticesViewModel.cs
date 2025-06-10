namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class PostcodeForServiceOfNoticesViewModel
{
    public PostcodeForServiceOfNoticesViewModel()
    {

    }

    public PostcodeForServiceOfNoticesViewModel(string postcode)
    {
        Postcode = postcode;
    }

    public string? Postcode { get; set; }
}
