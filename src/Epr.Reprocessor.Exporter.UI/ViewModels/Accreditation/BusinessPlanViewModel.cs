using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class BusinessPlanViewModel
    {
        public int InfrastructurePercentage { get; set; }
        public int PackagingWastePercentage { get; set; }
        public int BusinessCollections { get; set; }
        public int CommunicationsPercentage { get; set; }
        public int NewMarketsPercentage { get; set; }
        public int NewUsesForRecycledWastePercentage { get; set; }
        public int Total => InfrastructurePercentage
            + PackagingWastePercentage
            + BusinessCollections
            + CommunicationsPercentage
            + NewMarketsPercentage
            + NewUsesForRecycledWastePercentage;
    }
}
