using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

namespace Epr.Reprocessor.Exporter.UI.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class HomeViewModel : LinksConfig
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? OrganisationName { get; set; }
        public string? OrganisationNumber { get; set; }
        public List<RegistrationDataViewModel> RegistrationData { get; set; } = new();
        public List<AccreditationDataViewModel> AccreditationData { get; set; } = new();
    }

    public class RegistrationDataViewModel
    {
        public string Material { get; set; }
        public string SiteAddress { get; set; }
        public RegistrationStatus RegistrationStatus { get; set; }
        public int Year { get; set; }
        public string RegistrationContinueLink { get; set; }
        public ApplicationType ApplicationType { get; set; }
    }

    public class AccreditationDataViewModel
    {
        public string Material { get; set; }
        public string SiteAddress { get; set; }
        public Enums.AccreditationStatus AccreditationStatus { get; set; }
        public int Year { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public string AccreditationContinueLink { get; set; }
        public string AccreditationStartLink { get; set; }
    }
}
