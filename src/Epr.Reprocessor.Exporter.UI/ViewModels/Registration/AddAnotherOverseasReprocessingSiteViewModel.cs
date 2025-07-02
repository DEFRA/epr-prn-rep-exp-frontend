using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Resources;
using System.Security.Policy;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

[ExcludeFromCodeCoverage]
public class AddAnotherOverseasReprocessingSiteViewModel
{
    [Required]
    public bool? AddOverseasSiteAccepted { get; set; }

}


