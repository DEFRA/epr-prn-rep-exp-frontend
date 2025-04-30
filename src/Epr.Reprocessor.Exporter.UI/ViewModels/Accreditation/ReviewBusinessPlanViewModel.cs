using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class ReviewBusinessPlanViewModel
    {
        public int? InfrastructurePercentage { get; set; }
        public string? InfrastructureNotes { get; set; }

        public int? PriceSupportPercentage { get; set; }
        public string? PriceSupportNotes { get; set; }

        public int? BusinessCollectionsPercentage { get; set; }
        public string? BusinessCollectionsNotes { get; set; }

        public int? CommunicationsPercentage { get; set; }
        public string? CommunicationsNotes { get; set; }

        public int? DevelopingMarketsPercentage { get; set; }
        public string? DevelopingMarketsNotes { get; set; }

        public int? DevelopingNewUsesPercentage { get; set; }
        public string? DevelopingNewUsesNotes { get; set; }

    }
}
