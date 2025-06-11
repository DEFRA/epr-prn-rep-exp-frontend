using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.Enums;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Shared.Partials;
using Epr.Reprocessor.Exporter.UI.Validations.Attributes;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

/// <summary>
/// View model for Waste Carrier, Broker or Dealer registration number input.
/// </summary>
[ExcludeFromCodeCoverage]
public class WasteCarrierBrokerDealerRefViewModel
{
    /// <summary>
    /// The registration number of the Wast Carrier, Broker or Dealer
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(WasteCarrierBrokerDealer), ErrorMessageResourceName = "no_carrier_broker_dealer_registration_number_provided")]
    public virtual string? WasteCarrierBrokerDealerRegistrationNumber { get; set; }
}