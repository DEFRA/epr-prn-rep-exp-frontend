namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

[ExcludeFromCodeCoverage]
public record UpdateRegistrationTaskStatusDto
{
    public string TaskName { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}