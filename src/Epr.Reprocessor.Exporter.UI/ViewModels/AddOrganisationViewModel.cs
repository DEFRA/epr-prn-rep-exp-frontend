namespace Epr.Reprocessor.Exporter.UI.ViewModels
{
    public class AddOrganisationViewModel
    {
        public string FullName => $"{FirstName} {Lastname}";
        public string AddOrganisationLink { get; set; }
        public string FirstName { get; internal set; }
        public string Lastname { get; internal set; }
    }
}
