using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration
{
    [ExcludeFromCodeCoverage]
    public class SelectAuthorisationTypeViewModel
    {
        public List<AuthorisationTypes> AuthorisationTypes { get; set; }

        [Required(ErrorMessageResourceName = "error_message_no_selection", ErrorMessageResourceType = typeof(SelectAuthorisationType))]
        public int? SelectedAuthorisation { get; set; }

        public string? NationCode { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class AuthorisationTypes
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Label { get; set; }
        public string? SelectedAuthorisationText { get; set; }
        public List<string>? NationCodeCategory { get; set; }
    }
}
