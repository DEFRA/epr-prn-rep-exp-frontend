namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

public class UpdateRegistrationSiteAddressDto
{
    /// <summary>
    /// Gets or sets the identifier for the reprocessing site address
    /// </summary>
    public AddressDto ReprocessingSiteAddress { get; set; }

    /// <summary>
    /// Gets or sets the identifier for the legal document address
    /// </summary>
    public AddressDto LegalDocumentAddress { get; set; }
}
