using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

namespace Epr.Reprocessor.Exporter.UI.ViewModels
{
    public class HomeViewModel : LinksConfig
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationNumber { get; set; }
        public List<RegistrationDataViewModel> RegistrationData { get; set; } = new();
        public List<AccreditationDataViewModel> AccreditationData { get; set; } = new();
    }

    public class RegistrationDataViewModel
    {
        public string Material { get; set; }
        public string SiteAddress { get; set; }
        public string RegistrationStatus { get; set; }
        public string Action { get; set; }
    }

    public class AccreditationDataViewModel
    {
        public string Material { get; set; }
        public string SiteAddress { get; set; }
        public string AccreditationStatus { get; set; }
        public string Action { get; set; }
    }
}
