using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter.Test;

public class TestExporterSessionViewModel
{
    [Required(ErrorMessage = "Enter a registration ID")]
    [Display(Name = "Registration ID")]
    [RegularExpression(@"^(\{)?[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\})?$",
        ErrorMessage = "Enter a valid GUID")]
    public string RegistrationId { get; set; }

    [Required(ErrorMessage = "Enter a registration material ID")]
    [Display(Name = "Registration Material ID")]
    [RegularExpression(@"^(\{)?[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\})?$",
        ErrorMessage = "Enter a valid GUID")]
    public string RegistrationMaterialId { get; set; }
}
