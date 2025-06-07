using Epr.Reprocessor.Exporter.UI.App.Enums.Accreditation;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

public class CreateRegistrationDto
{
    public ApplicationType ApplicationTypeId { get; set; }

    public Guid OrganisationId { get; set; }
}
