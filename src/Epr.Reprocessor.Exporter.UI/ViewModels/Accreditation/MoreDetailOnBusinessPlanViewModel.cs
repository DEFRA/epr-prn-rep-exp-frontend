using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class MoreDetailOnBusinessPlanViewModel
    {
        public bool ShowInfrastructure { get; set; } = false;
        public bool ShowPriceSupport { get; set; } = false;
        public bool ShowBusinessCollections { get; set; } = false;
        public bool ShowCommunications { get; set; } = false;
        public bool ShowNewMarkets { get; set; } = false;
        public bool ShowNewUses { get; set; } = false;

        [MaxLength(300)]
        public string Infrastructure { get; set; } = string.Empty;

        [MaxLength(300)]
        public string PriceSupport { get; set; } = string.Empty;

        [MaxLength(300)]
        public string BusinessCollections { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Communications { get; set; } = string.Empty;

        [MaxLength(300)]
        public string NewMarkets { get; set; } = string.Empty;

        [MaxLength(300)]
        public string NewUses { get; set; } = string.Empty;
    }
}
