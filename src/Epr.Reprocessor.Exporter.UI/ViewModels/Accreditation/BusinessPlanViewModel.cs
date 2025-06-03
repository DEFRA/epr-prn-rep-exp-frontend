using System.ComponentModel.DataAnnotations;
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
        public string? InfrastructurePercentage { get; set; }
        public string? PackagingWastePercentage { get; set; }
        public string? BusinessCollectionsPercentage { get; set; }
        public string? CommunicationsPercentage { get; set; }
        public string? NewMarketsPercentage { get; set; }
        public string? NewUsesPercentage { get; set; }
        public string? OtherPercentage { get; set; }
        public string? TotalEntered { get; set; }

        private static bool IsWholeNumber(string input, out int number)
        {
            number = 0;
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return int.TryParse(input, out number) && input.All(char.IsDigit);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var resourceManager = ViewResources.BusinessPlan.ResourceManager;
            var percentages = new Dictionary<string, string>
            {
                { nameof(InfrastructurePercentage), InfrastructurePercentage },
                { nameof(PackagingWastePercentage), PackagingWastePercentage },
                { nameof(BusinessCollectionsPercentage), BusinessCollectionsPercentage },
                { nameof(CommunicationsPercentage), CommunicationsPercentage },
                { nameof(NewMarketsPercentage), NewMarketsPercentage },
                { nameof(OtherPercentage), OtherPercentage },
                { nameof(NewUsesPercentage), NewUsesPercentage }
            };

            bool anyValueProvided = false;
            int total = 0;

            foreach (var kvp in percentages)
            {
                var fieldName = kvp.Key;
                var value = kvp.Value;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    anyValueProvided = true;

                    if (decimal.TryParse(value, out decimal decVal))
                    {
                        if (decVal < 0)
                        {
                            yield return new ValidationResult(
                                resourceManager.GetString("error_must_be_positive"),
                                new[] { fieldName });

                            continue;
                        }

                        if (decVal > 100)
                        {
                            yield return new ValidationResult(
                                resourceManager.GetString("error_must_be_100_or_less"),
                                new[] { fieldName });

                            continue;
                        }

                        if (decVal % 1 != 0)
                        {
                            yield return new ValidationResult(resourceManager.GetString("error_whole_percentage"), new[] { fieldName });

                            continue;
                        }

                        if (!value.All(char.IsDigit))
                        {
                            yield return new ValidationResult(resourceManager.GetString("error_percentage_in_numbers"), new[] { fieldName });

                            continue;
                        }

                        total += (int)decVal;
                    }
                    else
                    {
                        yield return new ValidationResult(resourceManager.GetString("error_percentage_in_numbers"), new[] { fieldName });
                    }
                }
            }

            if (!anyValueProvided)
            {
                yield return new ValidationResult(
                    string.Format(resourceManager.GetString("error_enter_at_least_one_value"), Subject),
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
