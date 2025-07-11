using Epr.Reprocessor.Exporter.UI.Resources.Views.Enums;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.Enums;

/// <summary>
/// Defines the options for the frequency  which the weight limit for the permit is set for.
/// </summary>
public enum MaterialFrequencyOptions
{
    /// <summary>
    /// Default value for nothing is selected.
    /// </summary>
    None = 0,

    /// <summary>
    /// The weight limit is set for the whole year.
    /// </summary>
    [Display(Name = "PerYear", ResourceType = typeof(MaterialFrequencyOptionsResource))]
    PerYear = 1,

    /// <summary>
    /// The weight limit is set for a month.
    /// </summary>
    [Display(Name = "PerMonth", ResourceType = typeof(MaterialFrequencyOptionsResource))]
    PerMonth = 2,

    /// <summary>
    /// The weight limit is set for a week.
    /// </summary>
    [Display(Name = "PerWeek", ResourceType = typeof(MaterialFrequencyOptionsResource))]
    PerWeek = 3
}