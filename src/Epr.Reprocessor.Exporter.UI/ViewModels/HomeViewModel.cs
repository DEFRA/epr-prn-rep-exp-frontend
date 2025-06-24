using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;
using Epr.Reprocessor.Exporter.UI.ViewModels.Team;

namespace Epr.Reprocessor.Exporter.UI.ViewModels
{
    public class HomeViewModel : LinksConfig
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationNumber { get; set; }
        public TeamViewModel TeamViewModel { get; set; }
    }
}
