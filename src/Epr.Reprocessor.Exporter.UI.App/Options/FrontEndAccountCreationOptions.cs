using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.Options;

[ExcludeFromCodeCoverage]
public class FrontEndAccountCreationOptions
{
    public const string ConfigSection = "FrontEndAccountCreation";

    [Required]
    public string CreateUser { get; set; }
    public string AddOrganisation { get; set; }
}