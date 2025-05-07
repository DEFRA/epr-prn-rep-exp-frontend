namespace Epr.Reprocessor.Exporter.UI.Enums;

/// <summary>
/// Defines the options for the frequency  which the weight limit for the permit is set for.
/// </summary>
public enum PpcPermitFrequencyOptions
{
    /// <summary>
    /// Default value for nothing is selected.
    /// </summary>
    None = 0,

    /// <summary>
    /// The weight limit is set for the whole year.
    /// </summary>
    PerYear = 1,

    /// <summary>
    /// The weight limit is set for a month.
    /// </summary>
    PerMonth = 2,

    /// <summary>
    /// The weight limit is set for a week.
    /// </summary>
    PerWeek = 3
}