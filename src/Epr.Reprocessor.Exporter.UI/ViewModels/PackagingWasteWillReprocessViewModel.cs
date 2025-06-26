using Epr.Reprocessor.Exporter.UI.TagHelpers;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class PackagingWasteWillReprocessViewModel
{
	public List<CheckboxItem> Materials { get; set; } = [];

	[PackagingWasteRequiredSelectedAttribute(ErrorMessage = "Select the packaging waste you’ll reprocess")]
	public List<string> SelectedRegistrationMaterials { get; set; } = [];
	public int Year { get; set; }
	public List<CheckboxItem> MapForView(IList<MaterialLookupDto> materials)
	{
		foreach (var material in materials)
		{
			Materials.Add(new()
			{
				Value = material.Name.ToString(),
				LabelText = material.DisplayText
			});
		}

		return Materials;
	}
}