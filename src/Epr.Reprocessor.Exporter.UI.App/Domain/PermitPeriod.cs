using Epr.Reprocessor.Exporter.UI.App.Resources.Enums;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.App.Domain;

/// <summary>
/// Defines values for the permit period.
/// </summary>
public enum PermitPeriod
{
    /// <summary>
    /// Default value for nothing is selected.
    /// </summary>
    None = 0,

    /// <summary>
    /// The weight limit is set for the whole year.
    /// </summary>
    [Display(Name = "PerYear", ResourceType = typeof(PermitPeriodResource))]
    PerYear = 1,

    /// <summary>
    /// The weight limit is set for a month.
    /// </summary>
    [Display(Name = "PerMonth", ResourceType = typeof(PermitPeriodResource))]
    PerMonth = 2,

    /// <summary>
    /// The weight limit is set for a week.
    /// </summary>
    [Display(Name = "PerWeek", ResourceType = typeof(PermitPeriodResource))]
    PerWeek = 3
}