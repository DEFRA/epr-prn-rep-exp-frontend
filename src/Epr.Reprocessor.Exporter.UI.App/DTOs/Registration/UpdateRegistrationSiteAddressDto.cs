using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

[ExcludeFromCodeCoverage]
public class UpdateRegistrationSiteAddressDto
{
    /// <summary>
    /// Gets or sets the identifier for the reprocessing site address
    /// </summary>
    public AddressDto ReprocessingSiteAddress { get; set; }
}
