using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class SubmitAccreditationApplicationViewModel
{
    public bool IsApprovedUser { get; set; }

    public Guid AccreditationId { get; set; } = Guid.NewGuid();

    public PeopleAbleToSubmitApplication PeopleCanSubmitApplication { get; set; }
}

public class PeopleAbleToSubmitApplication
{
    public List<string> ApprovedPersons { get; set; }
}
