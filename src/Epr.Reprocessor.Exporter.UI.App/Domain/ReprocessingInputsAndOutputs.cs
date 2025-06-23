using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.Domain;

public class ReprocessingInputsAndOutputs
{
	public List<RegistrationMaterialDto> Materials { get; set; } = new();

	public List<MaterialItem> SelectedMaterials { get; set; } = new();

	public RegistrationMaterialDto? CurrentMaterial { get; set; }
}
