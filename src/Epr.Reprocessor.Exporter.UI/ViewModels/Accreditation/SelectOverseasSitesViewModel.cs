using System.ComponentModel.DataAnnotations;
using ViewResources = Epr.Reprocessor.Exporter.UI.Resources.Views.Accreditation;


namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class SelectOverseasSitesViewModel : IValidatableObject
    {
        public string Subject { get; set; } = "PERN";        
        public string? Action { get; set; }
        public Guid AccreditationId { get; set; } = Guid.Empty;
        public List<SelectListItem> OverseasSites { get; set; } = new();

        public List<string> SelectedOverseasSites { get; set; } = new();        

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (SelectedOverseasSites == null || SelectedOverseasSites.Count == 0)  
            {
                yield return new ValidationResult(
                ViewResources.SelectOverseasSites.ResourceManager.GetString("error_message_perns"),
                    [nameof(SelectedOverseasSites)]
                );
            }
        }
    }
}