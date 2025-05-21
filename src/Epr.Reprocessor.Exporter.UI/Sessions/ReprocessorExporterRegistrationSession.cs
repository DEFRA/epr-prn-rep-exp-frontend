using System.Diagnostics.CodeAnalysis;
using EPR.Common.Authorization.Interfaces;
using EPR.Common.Authorization.Models;

namespace Epr.Reprocessor.Exporter.UI.Sessions;

[ExcludeFromCodeCoverage]
public class ReprocessorExporterRegistrationSession : IHasUserData
{
    public UserData UserData { get; set; } = new();
    public List<string> Journey { get; set; } = new();
    public int? RegistrationId { get; set; }

    //TODO: Check this session in RPD and confirm if we can base our session on it
    //public RegistrationApplicationSession RegistrationApplicationSession { get; set; } = new ();
}