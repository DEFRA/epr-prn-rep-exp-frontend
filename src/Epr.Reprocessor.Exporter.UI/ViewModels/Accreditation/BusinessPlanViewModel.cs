using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using ViewResources = Epr.Reprocessor.Exporter.UI.Resources.Views.Accreditation;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class BusinessPlanViewModel : IValidatableObject
    {
        public Guid ExternalId { get; set; }
        public string? Action { get; set; }
        public string MaterialName { get; set; }
        public string Subject { get; set; }
        public decimal? InfrastructurePercentage { get; set; }
        public decimal? PackagingWastePercentage { get; set; }
        public decimal? BusinessCollectionsPercentage { get; set; }
        public decimal? CommunicationsPercentage { get; set; }
        public decimal? NewMarketsPercentage { get; set; }
        public decimal? NewUsesPercentage { get; set; }
        public decimal? TotalEntered => (decimal?)(InfrastructurePercentage
            + PackagingWastePercentage
            + BusinessCollectionsPercentage
            + CommunicationsPercentage
            + NewMarketsPercentage
            + NewUsesPercentage);

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var resourceManager = ViewResources.BusinessPlan.ResourceManager;

            var percentages = new Dictionary<string, decimal?>
            {
                { nameof(InfrastructurePercentage), InfrastructurePercentage },
                { nameof(PackagingWastePercentage), PackagingWastePercentage },
                { nameof(BusinessCollectionsPercentage), BusinessCollectionsPercentage },
                { nameof(CommunicationsPercentage), CommunicationsPercentage },
                { nameof(NewMarketsPercentage), NewMarketsPercentage },
                { nameof(NewUsesPercentage), NewUsesPercentage }
            };

            bool anyValueProvided = false;
            decimal total = 0;

            foreach (var kvp in percentages)
            {
                var fieldName = kvp.Key;
                var value = kvp.Value;

                if (value.HasValue)
                {
                    anyValueProvided = true;

                    if (value < 0)
                    {
                        yield return new ValidationResult(
                            resourceManager.GetString("error_must_be_positive"),
                            new[] { fieldName });
                    }

                    if (value > 100)
                    {
                        yield return new ValidationResult(
                            resourceManager.GetString("error_must_be_100_or_less"),
                            new[] { fieldName });
                    }

                    if (value % 1 != 0)
                    {
                        yield return new ValidationResult(
                            resourceManager.GetString("error_whole_percentage"),
                            new[] { fieldName });
                    }

                    total += value.Value;
                }
            }

            if (!anyValueProvided)
            {
                yield return new ValidationResult(
                    resourceManager.GetString("error_enter_at_least_one_value"),
                    percentages.Keys.ToArray());
            }

            if (total != 100)
            {
                yield return new ValidationResult(
                    resourceManager.GetString("error_total_must_be_100"),
                    new[] { nameof(TotalEntered) });
            }
        }
    }
}
