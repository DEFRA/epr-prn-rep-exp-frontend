using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.Resources.Views.ExporterJourney.WasteCarrierBrokerDealerReference;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;

/// <summary>
/// View model for Waste Carrier, Broker or Dealer registration number input.
/// </summary>
[ExcludeFromCodeCoverage]
public class WasteCarrierBrokerDealerRefViewModel: ExporterViewModelBase
{
    public Guid CarrierBrokerDealerPermitId { get; set; }

    /// <summary>
    /// The registration number of the Wast Carrier, Broker or Dealer
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(WasteCarrierBrokerDealerReference), ErrorMessageResourceName = "no_carrier_broker_dealer_registration_number_provided")]
    [MaxLength(15, ErrorMessageResourceType = typeof(WasteCarrierBrokerDealerReference), ErrorMessageResourceName = "carrier_broker_dealer_registration_number_exceeds_length")]
    public virtual string? WasteCarrierBrokerDealerRegistration { get; set; }    
}