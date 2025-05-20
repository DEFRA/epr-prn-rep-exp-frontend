using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using ViewResources = Epr.Reprocessor.Exporter.UI.Resources.Views.Accreditation;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class MoreDetailOnBusinessPlanViewModel
    {
        public Guid ExternalId { get; set; }
        public string Subject { get; set; } = "PRN";
        public string FormPostRouteName { get; set; }
        public string? Action { get; set; }

        public bool ShowInfrastructure { get; set; } = false;
        public bool ShowPriceSupport { get; set; } = false;
        public bool ShowBusinessCollections { get; set; } = false;
        public bool ShowCommunications { get; set; } = false;
        public bool ShowNewMarkets { get; set; } = false;
        public bool ShowNewUses { get; set; } = false;

        [MaxLength(500, ErrorMessageResourceName = "Infrastructure_max_length_error", ErrorMessageResourceType = typeof(ViewResources.MoreDetailOnBusinessPlan))]
        public string? Infrastructure { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessageResourceName = "PriceSupport_max_length_error", ErrorMessageResourceType = typeof(ViewResources.MoreDetailOnBusinessPlan))]
        public string? PriceSupport { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessageResourceName = "BusinessCollections_max_length_error", ErrorMessageResourceType = typeof(ViewResources.MoreDetailOnBusinessPlan))]
        public string? BusinessCollections { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessageResourceName = "Communications_max_length_error", ErrorMessageResourceType = typeof(ViewResources.MoreDetailOnBusinessPlan))]
        public string? Communications { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessageResourceName = "NewMarkets_max_length_error", ErrorMessageResourceType = typeof(ViewResources.MoreDetailOnBusinessPlan))]
        public string? NewMarkets { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessageResourceName = "NewUses_max_length_error", ErrorMessageResourceType = typeof(ViewResources.MoreDetailOnBusinessPlan))]
        public string? NewUses { get; set; } = string.Empty;
    }
}
