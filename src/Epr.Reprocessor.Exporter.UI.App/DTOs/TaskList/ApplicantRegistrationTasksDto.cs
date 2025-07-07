namespace Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;

/// <summary>
/// Defines a dto to carry the details of the tasks associated with the registration.
/// </summary>
[ExcludeFromCodeCoverage]
public class ApplicantRegistrationTasksDto
{
    /// <summary>
    /// The unique identifier for the registration.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The unique identifier for the organisation.
    /// </summary>
    public Guid OrganisationId { get; set; }

    /// <summary>
    /// Collection of registration tasks that are not material specific.
    /// </summary>
    public List<ApplicantRegistrationTaskDto> Tasks { get; set; } = [];

    /// <summary>
    /// Collection of materials with their associated tasks.
    /// </summary>
    public List<ApplicantRegistrationMaterialTasksDto> Materials { get; set; } = [];
}