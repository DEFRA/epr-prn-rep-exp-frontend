using FluentValidation.Results;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

public interface IValidationService
{
    Task<ValidationResult> ValidateAsync<T>(T instance, CancellationToken cancellationToken = default);
}