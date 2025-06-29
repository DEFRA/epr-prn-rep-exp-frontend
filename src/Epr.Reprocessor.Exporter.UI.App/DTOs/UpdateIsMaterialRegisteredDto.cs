namespace Epr.Reprocessor.Exporter.UI.App.DTOs;

[ExcludeFromCodeCoverage]
public class UpdateIsMaterialRegisteredDto
{
	public Guid RegistrationMaterialId { get; set; }
	public bool? IsMaterialRegistered { get; set; }
}
