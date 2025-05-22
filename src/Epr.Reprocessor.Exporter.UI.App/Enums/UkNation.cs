using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.App.Resources.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.Enums;

/// <summary>
/// The nation of the UK that an address/organisation is within.
/// </summary>
public enum UkNation
{
    /// <summary>
    /// No nation set.
    /// </summary>
    None = 0,

    /// <summary>
    /// The address is in England.
    /// </summary>
    [Display(Name = "England", ResourceType = typeof(UkNationResource))]
    England,

    /// <summary>
    /// The address is in Northern Ireland.
    /// </summary>
    [Display(Name = "NorthernIreland", ResourceType = typeof(UkNationResource))]
    NorthernIreland,

    /// <summary>
    /// The address is in Wales.
    /// </summary>
    [Display(Name = "Scotland", ResourceType = typeof(UkNationResource))]
    Scotland,

    /// <summary>
    /// The address is in Wales.
    /// </summary>
    [Display(Name = "Wales", ResourceType = typeof(UkNationResource))]
    Wales
}