using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Property)]
public class PackagingWasteRequiredSelectedAttribute : ValidationAttribute
{
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		var items = value as List<string>;
		if(items != null && items.Count > 0)
		{
			return ValidationResult.Success;
		}

		return new ValidationResult(ErrorMessage);
	}
}
