using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;

/// <summary>
/// Defines a list of tasks for the reprocessor journey.
/// </summary>
public enum TaskType
{
    /// <summary>
    /// The details of the reprocessing site and contact details.
    /// </summary>
    [Display(Name = "Site address and contact details")]
    SiteAddressAndContactDetails = 1,

    /// <summary>
    /// All permits and exemptions applicable for the site per material.
    /// </summary>
    [Display(Name = "Waste licenses, permits and exemptions")]
    WasteLicensesPermitsAndExemptions = 2,

    /// <summary>
    /// Further information about the reprocessing site.
    /// </summary>
    [Display(Name = "Reprocessing inputs and outputs")]
    ReprocessingInputsAndOutputs = 3,

    /// <summary>
    /// Details about the plan for inspection and sampling.
    /// </summary>
    [Display(Name = "Sampling and inspection plan per material")]
    SamplingAndInspectionPlan = 4,

    /// <summary>
    /// Unknown.
    /// </summary>
    Unknown = 99
}