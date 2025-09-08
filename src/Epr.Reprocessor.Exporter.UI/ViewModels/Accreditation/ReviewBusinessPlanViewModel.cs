using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class ReviewBusinessPlanViewModel
    {
        public Guid AccreditationId { get; set; } = Guid.Empty;
        public int ApplicationTypeId { get; set; } = 0;
        public string Subject { get; set; } = string.Empty;
        public string? Action { get; set; }

        public decimal? InfrastructurePercentage { get; set; }
        public decimal? PriceSupportPercentage { get; set; }
        public decimal? BusinessCollectionsPercentage { get; set; }
        public decimal? CommunicationsPercentage { get; set; }
        public decimal? NewMarketsPercentage { get; set; }
        public decimal? NewUsesPercentage { get; set; }
        public decimal? OtherPercentage { get; set; }

        public string? InfrastructureNotes { get; set; }
        public string? PriceSupportNotes { get; set; }
        public string? BusinessCollectionsNotes { get; set; }
        public string? CommunicationsNotes { get; set; }
        public string? NewMarketsNotes { get; set; }
        public string? NewUsesNotes { get; set; }
        public string? OtherNotes { get; set; }

        public string? BusinessPlanUrl { get; set; }
        public string? MoreDetailOnBusinessPlanUrl { get; set; }

    }
}
