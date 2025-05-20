using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class SubmitAccreditationApplicationViewModel
{
    public bool IsApprovedUser { get; set; }

    public PeopleAbleToSubmitApplication PeopleCanSubmitApplication { get; set; }
}

public class PeopleAbleToSubmitApplication
{
    public List<string> ApprovedPersons { get; set; }
}
