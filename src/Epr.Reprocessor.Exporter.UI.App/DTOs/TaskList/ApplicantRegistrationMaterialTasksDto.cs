namespace Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;

/// <summary>
/// Represents details of the tasks associated with a registration material.
/// </summary>
[ExcludeFromCodeCoverage]
public record ApplicantRegistrationMaterialTasksDto
{
    /// <summary>
    /// The unique identifier for the material entry.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The unique identifier of the registration that this material is registered for.
    /// </summary>
    public Guid RegistrationId { get; set; }

    /// <summary>
    /// Lookup details for the material that this registration is for.
    /// </summary>
    public MaterialLookupDto MaterialLookup { get; set; } = new();

    /// <summary>
    /// Lookup details for the status of the registration of this material.
    /// </summary>
    public MaterialStatusLookupDto? StatusLookup { get; set; }

    /// <summary>
    /// Flag to determine if the material is being registered for as part of the overall registration application.
    /// </summary>
    public bool IsMaterialRegistered { get; set; }

    /// <summary>
    /// Collection of tasks at the material level.
    /// </summary>
    public List<ApplicantRegistrationTaskDto> Tasks { get; set; } = new();
}