using Epr.Reprocessor.Exporter.UI.Resources.Views.Enums;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.Enums
{
    public enum MaterialFrequencyOptions
    {
        None = 0,
        [Display(Name = "PerYear", ResourceType = typeof(MaterialFrequencyOptionsResource))]
        PerYear,

        [Display(Name = "PerMonthly", ResourceType = typeof(MaterialFrequencyOptionsResource))]
        PerMonthly,

        [Display(Name = "PerWeekly", ResourceType = typeof(MaterialFrequencyOptionsResource))]
        PerWeekly,
    }
}
