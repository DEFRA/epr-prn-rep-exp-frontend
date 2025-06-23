namespace Epr.Reprocessor.Exporter.UI.ViewModels
{
    public class AddOrganisationViewModel
    {
        public string FullName => $"{FirstName} {LastName}";
        public string AddOrganisationLink { get; set; }
        public string ReadMoreAboutApprovedPersonLink { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
