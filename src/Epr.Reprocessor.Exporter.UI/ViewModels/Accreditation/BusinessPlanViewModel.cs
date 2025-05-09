using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class BusinessPlanViewModel
    {
        public string MaterialName { get; set; }
        public int InfrastructurePercentage { get; set; }
        public int PackagingWastePercentage { get; set; }
        public int BusinessCollectionsPercentage { get; set; }
        public int CommunicationsPercentage { get; set; }
        public int NewMarketsPercentage { get; set; }
        public int NewUsesPercentage { get; set; }
        public int Total => InfrastructurePercentage
            + PackagingWastePercentage
            + BusinessCollectionsPercentage
            + CommunicationsPercentage
            + NewMarketsPercentage
            + NewUsesPercentage;
    }
}
