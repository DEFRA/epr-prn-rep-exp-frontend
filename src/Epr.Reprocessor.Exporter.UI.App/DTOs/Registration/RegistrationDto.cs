namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration
{
    public class RegistrationDto
    {
        public Guid RegistrationId { get; set; }
        public int MaterialId { get; set; }
        public string Material {  get; set; }
        public int ApplicationTypeId { get; set; }
        public string ApplicationType { get; set; }
        public string RegistrationStatus { get; set; }
        public string AccreditationStatus { get; set; }
        public int? SiteId { get; set; }
        public AddressDto? SiteAddress { get; set; }
        public int Year { get; set; }
    }
}
