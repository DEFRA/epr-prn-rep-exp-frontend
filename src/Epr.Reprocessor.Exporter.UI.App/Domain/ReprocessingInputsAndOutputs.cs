namespace Epr.Reprocessor.Exporter.UI.App.Domain;

public class ReprocessingInputsAndOutputs
{
	public List<RegistrationMaterialDto> Materials { get; set; } = new();

	public RegistrationMaterialDto? CurrentMaterial { get; set; }

    public ReprocessingOutputDto? ReprocessingOutput { get; set; }

}